using shared;

public class AskLoadingEntered : ISerializable
{
    public bool Readied;
    public AskLoadingEntered() { }

    public AskLoadingEntered(bool readied) {  Readied = readied; }

    public override void Deserialize(Packet pPacket) { Readied = pPacket.ReadBool(); }

    public override void Serialize(Packet pPacket) { pPacket.Write(Readied); }
}
