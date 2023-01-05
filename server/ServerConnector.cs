using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class ServerConnector : MonoBehaviour
{
    public string ip = "127.0.0.1";
    public short port = 19132;

    public string name = "";
    public Guid playerGuid;

    public bool connected = false;

    PacketUtility utility = new PacketUtility();

    public Transform playerPrefab;
    public GameObject clientPlayer;

    public TMP_InputField ipText;
    public TMP_InputField portText;
    public TMP_InputField nameText;

    public PlayerManager manager;

    IPEndPoint ep;
    AsyncCallback recv = null;
    TcpClient client;

    BinaryReader reader;
    BinaryWriter writer;

    Vector3 serverPos;

    #region Player Variables
    public Vector3 oldAngles;
    public Vector3 oldScale;
    #endregion

    private void Update()
    {
        if(connected)
        {
            // Handle data being recieved / read.
            if (client.GetStream().DataAvailable)
            {
                byte id = reader.ReadByte();

                foreach (byte b in utility.packets.Keys)
                {
                    if (b == id)
                    {
                        Packet packet = utility.packets[b];

                        packet.SetReaderAndWriter(writer, reader);
                        packet.ReadPacket();

                        Debug.Log("Recieved " + packet.GetType().Name);

                        if (packet.GetType() == typeof(C2SMovementPacket))
                        {
                            C2SMovementPacket movePacket = (C2SMovementPacket)packet;
                            GameObject player = manager.GetPlayerTransformByName(movePacket.username).gameObject;

                            serverPos = movePacket.absolutePos.toVector3();

                            Rigidbody rb = player.GetComponent<Rigidbody>();

                            rb.drag = movePacket.currentDrag;

                            if (movePacket.isGrounded)
                            {
                                rb.AddForce(movePacket.moveDirection.toVector3().normalized * movePacket.moveSpeed * movePacket.movementMultiplier, ForceMode.Acceleration);
                            }
                            else if (!movePacket.isGrounded)
                            {
                                rb.AddForce(movePacket.moveDirection.toVector3().normalized * movePacket.moveSpeed * movePacket.movementMultiplier * movePacket.airMultiplier, ForceMode.Acceleration);
                            }

                            Vector3 yVal = rb.velocity;
                            yVal.y = (float)movePacket.velocity.y;

                            rb.velocity = yVal;

                            if (Vector3.Distance(
                                new Vector3(Mathf.Floor(player.transform.position.x), Mathf.Floor(player.transform.position.y), Mathf.Floor(player.transform.position.z)),
                                new Vector3(Mathf.Floor(movePacket.absolutePos.toVector3().x), Mathf.Floor(movePacket.absolutePos.toVector3().y), Mathf.Floor(movePacket.absolutePos.toVector3().z))) > .5f)
                            {
                                player.transform.position = movePacket.absolutePos.toVector3();
                            }
                        }

                        if (packet.GetType() == typeof(S2CPlayerSpawnPacket))
                        {
                            S2CPlayerSpawnPacket spawnPacket = (S2CPlayerSpawnPacket)packet;
                            manager.CreateUncontrollablePlayer((float)spawnPacket.x, (float)spawnPacket.y, (float)spawnPacket.z, spawnPacket.playerName);
                        }

                        if(packet.GetType() == typeof(C2SDisconnectPlayerPacket))
                        {
                            C2SDisconnectPlayerPacket disconnectPacket = (C2SDisconnectPlayerPacket)packet;
                            manager.RemovePlayer(disconnectPacket.playerName);
                        }
                    }
                }
            }

            // Handle data being sent.
            if (clientPlayer.GetComponent<Rigidbody>().velocity.magnitude != 0)
            {
                var movementPacket = new C2SMovementPacket(writer, reader);
                var movementScript = clientPlayer.GetComponent<PlayerMovement>();

                movementPacket.username = clientPlayer.name;
                movementPacket.moveDirection = Vector.fromVector3(movementScript.moveDirection);
                movementPacket.slopeMoveDirection = Vector.fromVector3(movementScript.slopeMoveDirection);
                movementPacket.velocity = Vector.fromVector3(movementScript.rb.velocity);
                movementPacket.absolutePos = Vector.fromVector3(clientPlayer.transform.position);
                movementPacket.movementMultiplier = movementScript.movementMultiplier;
                movementPacket.airMultiplier = movementScript.airMultiplier;
                movementPacket.moveSpeed = movementScript.moveSpeed;
                movementPacket.currentDrag = movementScript.rb.drag;
                movementPacket.isGrounded = movementScript.isGrounded;

                movementPacket.WritePacket();
            }
        }
    }

    void Start()
    {
        nameText.text = GenerateRandomName();
        client = new TcpClient();
    }

    public string GenerateRandomName()
    {
        return new string("UnityPlayer_" + UnityEngine.Random.Range(0000, 9999));
    }

    public void Connect()
    {
        this.name = nameText.text;

        string ip = this.ipText.text;
        int port = int.Parse(portText.text);
        Debug.Log("port is " + portText.text);

        GameObject.Find("Connect To Server").GetComponent<TextMeshProUGUI>().text = "Attempting To Connect To Server";

        this.ip = ip;
        this.port = (short)port;

        IPAddress address = null;
        bool IP = IPAddress.TryParse(ip, out address);

        if(!IP || address == null)
        {
            Task task = client.ConnectAsync(ip, port);
            int? tries = 0;

            while (!task.IsCompleted)
            {
                Debug.Log(string.Format("retrying to establish connection (x{0})", tries));
                tries++;

                Thread.Sleep(250);
            }

            if (!client.Connected)
            {
                Debug.LogError("could not establish connection");
                GameObject.Find("Connect To Server").GetComponent<TextMeshProUGUI>().text = "Connection Failed";
            }
        } else
        {
            Task task = client.ConnectAsync(IPAddress.Parse(ip), port);
            int? tries = 0;

            while (!task.IsCompleted)
            {
                Debug.Log(string.Format("retrying to establish connection (x{0})", tries));
                tries++;

                Thread.Sleep(250);
            }

            if (!client.Connected)
            {
                Debug.LogError("could not establish connection");
                GameObject.Find("Connect To Server").GetComponent<TextMeshProUGUI>().text = "Connection Failed";
                return;
            }
        }

        ipText.gameObject.SetActive(false);
        portText.gameObject.SetActive(false);
        nameText.gameObject.SetActive(false);
        GameObject.Find("ConnectButton").SetActive(false);

        GameObject.Find("Connect To Server").GetComponent<TextMeshProUGUI>().text = "Connecting to the server...";

        reader = new BinaryReader(client.GetStream());
        writer = new BinaryWriter(client.GetStream());

        GameObject.Find("Connect To Server").GetComponent<TextMeshProUGUI>().text = "Encrypting...";

        var handshake = new C2SHandshakePacket(writer, reader);

        handshake.ip = ip;
        handshake.port = (short)port;
        handshake.protocol = 2;

        handshake.WritePacket();

        var loginStart = new C2SLoginStartPacket(writer, reader);

        Debug.Log("Logging In...");
        GameObject.Find("Connect To Server").GetComponent<TextMeshProUGUI>().text = "Logging in...";

        loginStart.username = this.name;
        loginStart.guid = Guid.NewGuid().ToString();

        loginStart.WritePacket();

        var loginSuccess = new S2CLoginSuccessPacket(writer, reader);

        loginSuccess.ReadPacket();

        Guid guid = Guid.Parse(loginSuccess.guid);
        playerGuid = guid;

        string name = loginSuccess.username;

        bool playSwitch = loginSuccess.switchToPlayState;

        GameObject.Find("Connect To Server").GetComponent<TextMeshProUGUI>().text = "Loading...";

        var spawnData = new S2CLoadToGamePacket(writer, reader);

        spawnData.ReadPacket();

        int x = spawnData.x;
        int y = spawnData.y;
        int z = spawnData.z;

        long entityID = spawnData.entityID;

        var playerSpawnPacket = new S2CPlayerSpawnPacket(writer, reader);

        reader.ReadByte();
        playerSpawnPacket.ReadPacket();
        connected = true;

        clientPlayer = manager.CreatePlayer(x, y, z, name);
        client.NoDelay = true;
        client.ReceiveTimeout = 0;

        oldScale = clientPlayer.transform.localScale;
        oldAngles = new Vector3(0, 0, 0);

        Debug.Log(string.Format("Successfully Logged In At [{0},{1},{2}] With Entity ID {3}", new System.Object[]
        {
            x,
            y,
            z,
            entityID
        }));

        GameObject.Find("Connect To Server").GetComponent<TextMeshProUGUI>().text = "Connected!";
        GameObject.Find("Connect To Server").SetActive(false);
    }

    public void OnApplicationQuit()
    {
        if(connected)
        {
            var disconnectPacket = new C2SDisconnectPlayerPacket(writer, reader);

            disconnectPacket.playerGuid = playerGuid.ToString();
            disconnectPacket.playerName = name;
            disconnectPacket.reason = "player closed client";

            disconnectPacket.WritePacket();
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(serverPos, new Vector3(1, 2, 1));
        Gizmos.color = Color.blue;
    }
}