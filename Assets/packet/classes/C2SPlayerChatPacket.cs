using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

public class C2SPlayerChatPacket : Packet
{
    public C2SPlayerChatPacket(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
    {

    }

    public string username;
    public byte[] message;
    public int length;

    public override void ReadPacket()
    {
        username = ReadString();
        length = ReadInt();
        message = ReadBytes(length);
    }

    public override void WritePacket()
    {
        InsertByte((byte)0xE4);
        InsertString(username);
        InsertInt(length);
        InsertBytes(message);
        Send();
    }
}
