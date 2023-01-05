using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

public class C2SLoginStartPacket : Packet
{
    public C2SLoginStartPacket(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
    {

    }

    public string username;
    public string guid;

    public override void ReadPacket()
    {
        username = ReadString();
        guid = ReadString();
    }

    public override void WritePacket()
    {
        InsertString(username);
        InsertString(guid);
        Send();
    }
}