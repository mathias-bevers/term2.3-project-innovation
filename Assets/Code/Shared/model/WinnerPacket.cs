using shared;
using System.Collections.Generic;

public class WinnerPacket : ISerializable
{
    public List<int> winnerIDs { get; private set; } = new List<int>();
    public WinWay winway {get; private set; }
    public int Count;

    public WinnerPacket() { }
    public WinnerPacket(List<int> winnerIDs, WinWay winway)
    {
        this.winnerIDs = winnerIDs;
        this.winway = winway;
        Count = winnerIDs.Count;
    }

    public override void Deserialize(Packet pPacket)
    {
        winway = (WinWay)pPacket.ReadInt();
        Count = pPacket.ReadInt();
        for(int i = 0; i < Count; i++)
        {
            winnerIDs.Add(pPacket.ReadInt());
        }
    }

    public override void Serialize(Packet pPacket)
    {
        pPacket.Write((int)winway);
        pPacket.Write(Count);
        foreach(int i in winnerIDs)
        {
            pPacket.Write(i);
        }
    }
}

