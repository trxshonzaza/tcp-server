using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

public class C2SHandshakePacket : Packet
{
    public C2SHandshakePacket(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
    {

    }

    public int protocol;
    public string ip;
    public short port;
    public int status;

    public override void ReadPacket()
    {
        protocol = ReadInt();
        ip = ReadString();
        port = ReadShort();
        status = ReadInt();
    }

    public override void WritePacket()
    {
        InsertInt(0);
        InsertString(ip);
        InsertShort(port);
        InsertInt(protocol);
        Send();
    }
}
