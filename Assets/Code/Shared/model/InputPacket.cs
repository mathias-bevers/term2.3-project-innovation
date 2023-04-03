
using shared;

public class InputPacket : ISerializable
{
    public SerializableVector2 input;

    public InputPacket() { }
    public InputPacket(SerializableVector2 position)
    {
        this.input = position;
    }
    public InputPacket(float x, float y)
    {
        this.input = new SerializableVector2(x, y);
    }

    public override void Deserialize(Packet pPacket)
    {
        input = pPacket.Read<SerializableVector2>(); 
    }

    public override void Serialize(Packet pPacket)
    {
        pPacket.Write(input);
    }
}
