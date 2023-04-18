using shared;

public class RequestNameChange : ISerializable
{
    public int ID { get; set; }
    public string Name { get; set; }

    public RequestNameChange() { }

    public RequestNameChange(int ID, string newName)
    {
        this.ID = ID;
        this.Name = newName;
    }

    public override void Deserialize(Packet packet)
    {
        ID = packet.ReadInt();
        Name = packet.ReadString();
    }

    public override void Serialize(Packet packet)
    {
        packet.Write(ID);
        packet.Write(Name);
    }
}

