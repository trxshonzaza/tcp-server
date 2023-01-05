using System.IO;

public class S2CStatusPongPacket : Packet
{
    private BinaryWriter writer;
    private BinaryReader reader;

    public S2CStatusPongPacket(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
    {
        this.writer = writer;
        this.reader = reader;
    }

    public long playerCount,
        maxPlayers;

    public string description;


    public override void ReadPacket()
    {
        playerCount = ReadLong();
        maxPlayers = ReadLong();
        description = ReadString();
    }

    public override void WritePacket()
    {
        
    }
}