using shared;
using UnityEngine;

public class SerializableVector3 : ISerializable
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3() { }

    public SerializableVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public override void Deserialize(Packet pPacket)
    {
        x = pPacket.ReadFloat();
        y = pPacket.ReadFloat();
        z = pPacket.ReadFloat();
    }

    public override void Serialize(Packet pPacket)
    {
        pPacket.Write(x);
        pPacket.Write(y);
        pPacket.Write(z);
    }

    public static implicit operator SerializableVector3(Vector3 vector) => new SerializableVector3(vector.x, vector.y, vector.z);
    public static implicit operator Vector3(SerializableVector3 vector) => new Vector3(vector.x, vector.y, vector.z);
}
