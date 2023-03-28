
using shared;

public class Heartbeat : ISerializable
{
    public int num;

    public Heartbeat() { }

    public Heartbeat(int num) { this.num = num; }

    public override void Deserialize(Packet pPacket)
    {
        num = pPacket.ReadInt();
    }

    public override void Serialize(Packet pPacket)
    {
        pPacket.Write(num);
    }
}

