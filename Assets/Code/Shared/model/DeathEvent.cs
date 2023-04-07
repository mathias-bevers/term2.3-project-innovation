using shared;

public class DeathEvent : ISerializable
{
    public int ID { get; set; }
    public DeathType deathType { get; set; }

    public DeathEvent() { }
    public DeathEvent(int ID, DeathType deathType)
    {
        this.ID = ID;
        this.deathType = deathType;
    }

    public override void Deserialize(Packet pPacket)
    {
        ID = pPacket.ReadInt();
        deathType = (DeathType)pPacket.ReadInt();   
    }

    public override void Serialize(Packet pPacket)
    {
        pPacket.Write(ID);
        pPacket.Write((int)deathType);
    }
}

public enum DeathType
{
    Burned,
    Dirty
}

