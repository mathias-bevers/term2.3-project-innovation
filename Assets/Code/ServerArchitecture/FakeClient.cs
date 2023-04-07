using shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeClient : IRegistrable
{
    [HideInInspector]
    public int id;
    public override int ID { get => id; }
    public void SetID(int id) => this.id = id;

    public override PacketHandler.ClientReader reader { get; set; }

    MainServer server;

    private void Start()
    {
        server = FindObjectOfType<MainServer>();
    }

    public override void Register(PacketHandler.ClientReader reader)
    {
        
    }

    public override void SendPacket(ISerializable serializable)
    {
        if (server == null) return;
        ServerClient client = server.Server.GetClientByID(id);
        if(client == null) return;
        server.Server.ReceivedPacket(client, serializable);
    }

    public override void Unregister(PacketHandler.ClientReader reader)
    {
        
    }
}
