using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class Client
{
    BinaryReader reader;
    BinaryWriter writer;
    TcpClient client;

    string playerName;
    Guid playerGuid;

    public Vector position;
    public Vector rotation;
    public Vector scale;

    public Client(Server server, TcpClient client, string playerName, Guid playerGuid)
    {
        reader = new BinaryReader(client.GetStream());
        writer = new BinaryWriter(client.GetStream());

        this.playerName = playerName;
        this.playerGuid = playerGuid;
        this.client = client;

        position = new Vector();
        rotation = new Vector();
        scale = new Vector();
    }

    public bool isDataAvailable()
    {
        return client.GetStream().DataAvailable || reader.PeekChar() != -1;
    }

    public BinaryReader GetReader()
    {
        return reader;
    }

    public BinaryWriter GetWriter()
    {
        return writer;
    }

    public TcpClient GetClient()
    {
        return client;
    }

    public string getPlayerName()
    {
        return playerName;
    }

    public Guid getPlayerGuid()
    {
        return playerGuid;
    }

    public double GetXPos()
    {
        return position.x;
    }

    public double GetYPos()
    {
        return position.y;
    }

    public double GetZPos()
    {
        return position.z;
    }

    public double GetXRot()
    {
        return rotation.x;
    }

    public double GetYRot()
    {
        return rotation.y;
    }

    public double GetZRot()
    {
        return rotation.z;
    }

    public double GetXScale()
    {
        return scale.x;
    }

    public double GetYScale()
    {
        return scale.y;
    }

    public double GetZScale()
    {
        return scale.z;
    }
}