using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PacketUtility
{
    public Dictionary<byte, Packet> packets = new Dictionary<byte, Packet>();

    public PacketUtility()
    {
        AddPacketData();
    }

    private void AddPacketData()
    {
        packets.Add((byte)0x2A, new C2SMovementPacket(null, null));
        packets.Add((byte)0x1B, new S2CPlayerSpawnPacket(null, null));
        packets.Add((byte)0x1F, new C2SDisconnectPlayerPacket(null, null));
    }
}