using shared;

public class Disconnected : ISerializable
{
    public int ID;
    public Disconnected() { }

    public Disconnected(int disconnectedID) { ID = disconnectedID; }    

    public override void Deserialize(Packet pPacket) { ID = pPacket.ReadInt(); }

    public override void Serialize(Packet pPacket) { pPacket.Write(ID); }
}
