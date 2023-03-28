namespace shared
{
    /**
     * Sent from CLIENT 2 SERVER.
     */
    public class AddRequest : ISerializable
    {
        public Score score;

        public override void Serialize(Packet pPacket)
        {
            pPacket.Write(score);
        }

        public override void Deserialize(Packet pPacket)
        {
            score = pPacket.Read<Score>();
        }
    }
}
