using shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PacketHandler;

public interface IRegistrable
{
    public int ID { get; }
    public ClientReader reader { get; set; }
    public void Register(ClientReader reader);
    public void Unregister(ClientReader reader);
    public void SendPacket(ISerializable serializable);
}
