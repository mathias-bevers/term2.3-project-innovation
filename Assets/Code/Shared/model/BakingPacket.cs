using shared;

public class BakingPacket : ISerializable
{
    public float maxBake;
    public BakingPacketData[] bakingPackets;
    public int Count => bakingPackets.Length;

    public BakingPacket() { }
    public BakingPacket(float maxBake, BakingPacketData[] bakingPackets)
    {
        this.maxBake = maxBake;
        this.bakingPackets = bakingPackets;
    }

    public override void Deserialize(Packet pPacket)
    {
        maxBake = pPacket.ReadFloat();
        bakingPackets = new BakingPacketData[pPacket.ReadInt()];
        for(int i = 0; i < Count; i++)
        {
            bakingPackets[i] = new BakingPacketData(pPacket.ReadInt(), pPacket.ReadFloat());
        }
    }

    public override void Serialize(Packet pPacket)
    {
        pPacket.Write(maxBake);
        pPacket.Write(Count);
        foreach(BakingPacketData bakingPacket in bakingPackets)
        {
            pPacket.Write(bakingPacket.ID);
            pPacket.Write(bakingPacket.actualAmount);
        }
    }
}

public struct BakingPacketData
{
    public int ID;
    public float actualAmount;
    public BakingPacketData(int ID, float actualAmount)
    {
        this.ID = ID;
        this.actualAmount = actualAmount;
    }
}

