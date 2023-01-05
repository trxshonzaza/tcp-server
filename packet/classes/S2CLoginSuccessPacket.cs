using System.IO;

public class S2CLoginSuccessPacket : Packet
{
    public S2CLoginSuccessPacket(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
    {

    }

    public string guid;
    public string username;
    public bool switchToPlayState;

    public override void ReadPacket()
    {
        guid = ReadString();
        username = ReadString();
        switchToPlayState = ReadBool();
    }

    public override void WritePacket()
    {
        
    }
}