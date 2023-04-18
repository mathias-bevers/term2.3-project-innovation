using shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PacketHandler;

public class EchoClient : IRegistrable
{
    public override int ID { get; }

    public override PacketHandler.ClientReader reader { get; set; }

    private ServerClient client = new ServerClient(new TcpClient(), -1);
    public ServerClient Client { get => client; }

    void HandleReadPackets()
    {
        if (client == null) return;
        if (client.Available == 0) return;

        byte[] gottenBytes = StreamUtil.Read(client.stream);
        Packet packet = new Packet(gottenBytes);
        ISerializable current = packet.Read<ISerializable>();

        reader?.Invoke(client, current, TrafficDirection.Received);
    }

    public override void SendPacket(ISerializable serializable)
    {
        Packet packet = new Packet();
        packet.Write(serializable);
        try
        {
            reader?.Invoke(client, serializable, TrafficDirection.Send);
            StreamUtil.Write(client.stream, packet.GetBytes());
        }
        catch (Exception e) { Debug.LogError(e); }
    }

    void ReceivePacket(ServerClient client, ISerializable serializable, TrafficDirection trafficDirection)
    {
        if (trafficDirection != TrafficDirection.Received) return;
        if (serializable is Heartbeat) { SendPacket(serializable); }
    }


    void Awake() => DontDestroyOnLoad(this);
    void Start() => client.client.Connect(new IPEndPoint(Settings.ip, Settings.port));
    void Update() => HandleReadPackets();
    void OnEnable() => Register(ReceivePacket);
    void OnDisable() => Unregister(ReceivePacket);
    void OnDestroy() => SendPacket(new Disconnected());
    public override void Register(ClientReader reader) => this.reader += reader;
    public override void Unregister(ClientReader reader) => this.reader -= reader;
}
