
using shared;

public class Disconnected : ISerializable
{
    public int disconnectedID;

    public Disconnected() { }

    public Disconnected(int disconnectedID) { this.disconnectedID= disconnectedID; }    

    public override void Deserialize(Packet pPacket)
    {
        disconnectedID = pPacket.ReadInt();
    }

    public override void Serialize(Packet pPacket)
    {
        pPacket.Write(disconnectedID);
    }
}
