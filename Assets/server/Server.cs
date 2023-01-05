using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using System.Security.Cryptography;
using System.Text;
using ThreadPriority = System.Threading.ThreadPriority;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class Server : MonoBehaviour
{
    public AsyncCallback recv = null;

    public PacketUtility packetUtil;

    public Config config;

    public ByteState state;

    [SerializeField] public List<Client> clients = new List<Client>();
    [SerializeField] public List<Client> clientsToAdd = new List<Client>();
    [SerializeField] public List<Client> clientsToRemove = new List<Client>();

    public static Server currentInstance = null;

    public TcpListener tcp;

    public string ip;
    public short port;

    public bool running = false;
    
    public void Start()
    {
        try
        {
            config = Config.FromConfigPath(Application.dataPath);
            config.GenerateConfig();

            this.ip = config.ip;
            this.port = config.port;

            Thread.CurrentThread.Name = "Main Thread";

            Console.ForegroundColor = ConsoleColor.Green;
            Debug.Log("engine loaded!");
            Console.ForegroundColor = ConsoleColor.Gray;

            Debug.Log("Preparing server");

            tcp = new TcpListener(IPAddress.Parse(config.ip), config.port);
            tcp.Start();

            Thread connectionThread = new Thread(() => ListenForConnections());
            connectionThread.Name = "Connection Handler Thread";
            connectionThread.Priority = ThreadPriority.Lowest;
            connectionThread.Start();

            state = new ByteState();

            packetUtil = new PacketUtility();

            Debug.Log("Listening for connections on ip " + config.ip + " and port " + config.port);

            running = true;

            currentInstance = this;

        }catch(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Debug.Log("[CRITICAL] server failed to start \n" + e.Message + "\n" + e.StackTrace);

            if(config.generateCrashReports)
            {
                ServerCrashReport report = new ServerCrashReport(new StackTrace(e, true), "server startup", e.ToString(), e.Message, e.StackTrace, Thread.CurrentThread);
            }

            StopServer("server failed to start");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }

    public void Update()
    {
        if(running)
        {
            GameLogic(new ByteState());
            Console.Title = "Trxsh's TCP Server Framework [ Connected: " + clients.Count + "]";
        }
    }

    public void GameLogic(object state)
    {
        Client lastClientRecievedBy = null;
        Packet lastPacketRecievedBy = null;

        try
        {
            foreach (Client c in clients)
            {
                if (c.isDataAvailable())
                {
                    byte id = c.GetReader().ReadByte();

                    foreach (byte b in packetUtil.packets.Keys)
                    {
                        if (b == id)
                        {
                            var packet = packetUtil.packets[b];

                            packet.SetReaderAndWriter(c);
                            packet.ReadPacket();

                            if (packet.GetType() == typeof(C2SMovementPacket))
                            {
                                C2SMovementPacket movePacket = (C2SMovementPacket)packet;
                                c.position = movePacket.absolutePos;

                                if (lastClientRecievedBy != null && lastPacketRecievedBy != null)
                                {
                                    if (lastClientRecievedBy == c)
                                    {
                                        if (((C2SMovementPacket)lastPacketRecievedBy).moveDirection == movePacket.moveDirection ||
                                          ((C2SMovementPacket)lastPacketRecievedBy).slopeMoveDirection == movePacket.slopeMoveDirection)
                                        {
                                            lastPacketRecievedBy = packet;
                                            lastClientRecievedBy = c;

                                            return;
                                        }
                                    }
                                }

                                foreach (Client client in clients)
                                {
                                    if (client != c)
                                    {
                                        packet.SetReaderAndWriter(client);
                                        packet.WritePacket();
                                    }
                                }
                            }

                            if (packet.GetType() == typeof(S2CPlayerSpawnPacket))
                            {
                                S2CPlayerSpawnPacket spawnPacket = (S2CPlayerSpawnPacket)packet;

                                foreach (Client client in clients)
                                {
                                    packet.SetReaderAndWriter(client);
                                    packet.WritePacket();
                                }
                            }

                            if (packet.GetType() == typeof(C2SDisconnectPlayerPacket))
                            {
                                C2SDisconnectPlayerPacket disconnectPacket = (C2SDisconnectPlayerPacket)packet;

                                Debug.Log("player " + c.getPlayerName() + " disconnected due to " + disconnectPacket.reason);

                                clientsToRemove.Add(c);

                                foreach (Client client in clients)
                                {
                                    if (client != c)
                                    {
                                        packet.SetReaderAndWriter(client);
                                        packet.WritePacket();
                                    }
                                }
                            }

                            if (packet.GetType() == typeof(C2SPlayerChatPacket))
                            {
                                C2SPlayerChatPacket chatPacket = (C2SPlayerChatPacket)packet;

                                string message = Encoding.ASCII.GetString(chatPacket.message);

                                Debug.Log("[CHAT] " + chatPacket.username + ": " + message);

                                foreach (Client client in clients)
                                {
                                    if (client != c)
                                    {
                                        packet.SetReaderAndWriter(client);
                                        packet.WritePacket();
                                    }
                                }
                            }

                            lastPacketRecievedBy = packet;
                            lastClientRecievedBy = c;
                        }
                    }
                }
            }

            if (clientsToAdd.Count > 0)
            {
                clients.AddRange(clientsToAdd);
                clientsToAdd.Clear();
            }

            if(clientsToRemove.Count > 0)
            {
                for(int i = 0; i < clients.Count; i++)
                {
                    Client client = clients[i];

                    foreach(Client _client in clientsToRemove)
                    {
                        if(client.getPlayerGuid() == _client.getPlayerGuid())
                        {
                            clients.RemoveAt(i);
                            client.GetClient().Close();
                        }
                    }
                }
            }

        }
        catch(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Debug.LogError("[CRITICAL] failed to handle game logic" + "\n" + e.Message + "\n" + e.StackTrace);

            if(config.generateCrashReports)
            {
                ServerCrashReport report = new ServerCrashReport(new StackTrace(e, true), "Game Tick Loop", e.ToString(), e.Message, e.StackTrace, Thread.CurrentThread);
            }

            StopServer("failed to handle game logic");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }

    public void ListenForConnections()
    {
        tcp.BeginAcceptTcpClient(recv = (ar) =>
        {
            // Standard Initalizers To Begin Login Start/Status

            ByteState so = ar.AsyncState as ByteState;
            TcpClient toAccept = tcp.EndAcceptTcpClient(ar);
            tcp.BeginAcceptTcpClient(recv, so);
            
            // Login/Status Protocol

            try
            {
                BinaryReader tempReader = new BinaryReader(toAccept.GetStream());
                BinaryWriter tempWriter = new BinaryWriter(toAccept.GetStream());

                var handshake = new C2SHandshakePacket(tempWriter, tempReader);

                handshake.ReadPacket();

                //handshake check, depricated due to port forwarding and ip forwarding issues
                /*if(handshake.ip != this.ip || handshake.port != this.port)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Debug.Log("[WARNING] handshake from " + toAccept.Client.RemoteEndPoint + " refused due to ip/port mismatch (is the client genuine?)");
                    Console.ForegroundColor = ConsoleColor.Gray;

                    return;
                }*/

                if (handshake.protocol == 0)
                {
                    if (handshake.status == (int)Status.STATUS)
                    {
                        var status = new S2CStatusPongPacket(tempWriter, tempReader);

                        status.playerCount = clients.Count;
                        status.maxPlayers = this.config.maxPlayers;
                        status.description = this.config.description;

                        status.WritePacket();
                    }

                    if (handshake.status == (int)Status.LOGIN)
                    {
                        var loginStart = new C2SLoginStartPacket(tempWriter, tempReader);

                        loginStart.ReadPacket();

                        string name = loginStart.username;
                        Guid guid = Guid.Parse(loginStart.guid);

                        Debug.Log("GUID Of Connecting Player Is " + guid.ToString());

                        var loginSuccess = new S2CLoginSuccessPacket(tempWriter, tempReader);

                        loginSuccess.guid = guid.ToString();
                        loginSuccess.username = name;

                        // Switches Client To Play State
                        loginSuccess.switchToPlayState = true;

                        loginSuccess.WritePacket();

                        var playData = new S2CLoadToGamePacket(tempWriter, tempReader);

                        playData.x = 0;
                        playData.y = 2;
                        playData.z = 0;

                        playData.entityID = new System.Random().Next(-999999999, 999999999);

                        playData.WritePacket();

                        var playerSpawn = new S2CPlayerSpawnPacket(tempWriter, tempReader);

                        playerSpawn.playerName = name;
                        playerSpawn.x = playData.x;
                        playerSpawn.y = playData.y;
                        playerSpawn.z = playData.z;

                        playerSpawn.WritePacket();

                        Client client = new Client(this, toAccept, name, guid);

                        Debug.Log(String.Format("[{0}] {1} Successfully Logged In At Position [{2},{3},{4}] With Entity ID {5}", new object[]
                        {
                            toAccept.Client.RemoteEndPoint,
                            name,
                            playData.x,
                            playData.y,
                            playData.z,
                            playData.entityID
                        }));

                        // Spawn player for other clients
                        foreach (Client pClient in clients)
                        {
                            playerSpawn.SetReaderAndWriter(pClient);
                            playerSpawn.WritePacket();
                        }

                        // Spawn all other players for client joining
                        foreach (Client pClient in clients)
                        {
                            S2CPlayerSpawnPacket packet = new S2CPlayerSpawnPacket(tempWriter, tempReader);

                            packet.playerName = pClient.getPlayerName();
                            packet.x = pClient.GetXPos();
                            packet.y = pClient.GetYPos();
                            packet.z = pClient.GetZPos();

                            packet.WritePacket();
                        }

                        clientsToAdd.Add(client);
                        client.GetClient().NoDelay = true;
                        client.GetClient().ReceiveTimeout = 0;
                    }
                }
            }catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Debug.Log("[WARNING] failed to read/write packet(s)! is the client genuine? \n " + e.Message + "\n" + e.StackTrace);
                Console.ForegroundColor = ConsoleColor.Gray;

                toAccept.Dispose();
            }
        }, state);
    }

    public List<BinaryWriter> getAllWriterStreams()
    {
        List<BinaryWriter> writers = new List<BinaryWriter>();
        
        foreach(Client client in clients)
        {
            writers.Add(client.GetWriter());
        }

        return writers;
    }

    public List<BinaryReader> getAllReaderStreams()
    {
        List<BinaryReader> readers = new List<BinaryReader>();

        foreach (Client client in clients)
        {
            readers.Add(client.GetReader());
        }

        return readers;
    }

    public void StopServer(string reason)
    {
        running = false;

        Console.ForegroundColor = ConsoleColor.Yellow;
        Debug.Log("stopping server: " + reason);
        Console.ForegroundColor = ConsoleColor.Gray;

        foreach(Client client in clients)
        {
            C2SDisconnectPlayerPacket packet = new C2SDisconnectPlayerPacket(client.GetWriter(), client.GetReader());

            packet.playerGuid = client.getPlayerGuid().ToString();
            packet.playerName = client.getPlayerName();
            packet.reason = "server closed: " + reason;
        }

        tcp.Stop();

        Debug.Log("server stopped, please exit console window to continue...");
    }

    public void OnApplicationQuit()
    {
        if(running)
        {
            StopServer("server forcibly closed");
        }
    }
}
