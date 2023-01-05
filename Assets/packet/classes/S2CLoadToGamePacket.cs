using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

public class S2CLoadToGamePacket : Packet
{
    public S2CLoadToGamePacket(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
    {

    }

    public int x,
        y, z;
    public long entityID;

    public override void ReadPacket()
    {
        x = ReadInt();
        y = ReadInt();
        z = ReadInt();
        entityID = ReadLong();
    }

    public override void WritePacket()
    {
        InsertInt(x);
        InsertInt(y);
        InsertInt(z);
        InsertLong(entityID);
    }
}