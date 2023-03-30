using shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PacketHandler;

public abstract class IRegistrable : MonoBehaviour
{
    public abstract int ID { get; }
    public abstract ClientReader reader { get; set; }
    public abstract void Register(ClientReader reader);
    public abstract void Unregister(ClientReader reader);
    public abstract void SendPacket(ISerializable serializable);
}
