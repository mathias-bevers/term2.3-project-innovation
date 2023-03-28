﻿namespace shared
{
    /**
     * Simple player score value holder.
     */
    public class Score : ISerializable
    {
        public string name;
        public int score;

        public Score() { }

        public Score(string pName, int pScore)
        {
            name = pName;
            score = pScore;
        }

        public override void Serialize(Packet pPacket)
        {
            pPacket.Write(name);
            pPacket.Write(score);
        }

        public override void Deserialize(Packet pPacket)
        {
            name = pPacket.ReadString();
            score = pPacket.ReadInt();
        }
    }
}