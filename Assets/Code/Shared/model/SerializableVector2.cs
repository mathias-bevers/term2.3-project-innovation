using shared;
using UnityEngine;

public class SerializableVector2 : ISerializable
{
    public float x;
    public float y;

    public SerializableVector2() { }

    public SerializableVector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public override void Deserialize(Packet pPacket)
    {
        x = pPacket.ReadFloat();
        y = pPacket.ReadFloat();
    }

    public override void Serialize(Packet pPacket)
    {
        pPacket.Write(x);
        pPacket.Write(y);
    }

    public static implicit operator SerializableVector2(Vector2 vector) => new SerializableVector2(vector.x, vector.y);
    public static implicit operator Vector2(SerializableVector2 vector) => new Vector2(vector.x, vector.y);

    public new string ToString()
    {
        return $"Vector2: {x}:{y}";
    }
}
