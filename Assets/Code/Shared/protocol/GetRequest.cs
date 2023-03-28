namespace shared
{
    /**
     * Sent from CLIENT 2 SERVER.
     */
    public class GetRequest : ISerializable
    {
        public override void Serialize(Packet pPacket)   {}
        public override void Deserialize(Packet pPacket) {}
    }
}
