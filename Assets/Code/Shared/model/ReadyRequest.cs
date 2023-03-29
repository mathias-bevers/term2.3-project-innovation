using shared;

public class ReadyRequest : ISerializable
{
    public int ID;
    public bool Readied;
    public ReadyRequest() { }

    public ReadyRequest(int requestID, bool readied) { ID = requestID; Readied = readied; }

    public override void Deserialize(Packet pPacket) { ID = pPacket.ReadInt(); Readied = pPacket.ReadBool(); }

    public override void Serialize(Packet pPacket) { pPacket.Write(ID); pPacket.Write(Readied); }
}
