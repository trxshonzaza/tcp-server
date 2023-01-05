using System.IO;

public class S2CPlayerSpawnPacket : Packet
{
    public S2CPlayerSpawnPacket(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
    {

    }

    public byte id;

    public string playerName;

    public double x,
        y,
        z;

    public override void ReadPacket()
    {
        playerName = ReadString();
        x = ReadDouble();
        y = ReadDouble();
        z = ReadDouble();
    }

    public override void WritePacket()
    {
        InsertByte((byte)0x2B);
        InsertString(playerName);
        InsertDouble(x);
        InsertDouble(y);
        InsertDouble(z);
        Send();
    }
}