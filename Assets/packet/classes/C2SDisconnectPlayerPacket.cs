using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

class C2SDisconnectPlayerPacket : Packet
{
    public C2SDisconnectPlayerPacket(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
    {

    }

    public string playerGuid;
    public string playerName;
    public string reason;

    public override void ReadPacket()
    {
        playerGuid = ReadString();
        playerName = ReadString();
        reason = ReadString();
    }

    public override void WritePacket()
    {
        InsertByte((byte)0x1F);
        InsertString(playerGuid);
        InsertString(playerName);
        InsertString(reason);
        Send();
    }
}
