using shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using static PacketHandler;

public class UserClient : MonoBehaviour
{
    public ClientReader reader { get; set; }

    private IPAddress ipAd = IPAddress.Parse("127.0.0.1");
    private ServerClient client = new ServerClient(new TcpClient(), -1);

    void HandleReadPackets()
    {
        if (client.Available == 0) return;

        byte[] gottenBytes = StreamUtil.Read(client.stream);
        Packet packet = new Packet(gottenBytes);
        ISerializable current = packet.Read<ISerializable>();

        Type storedType = current.GetType();
        reader?.Invoke(client, current);
    }

    void ReceivePacket(ServerClient client, ISerializable serializable)
    {
        if (serializable is Heartbeat) SendPacket(serializable);
        Debug.Log("Received a undeclared packet: " + serializable.GetType());
    }

    public void SendPacket(ISerializable serializable)
    {
        Packet packet = new Packet();
        packet.Write(serializable);
        try
        {
            StreamUtil.Write(client.stream, packet.GetBytes());
        }
        catch { }
    }

    void Awake() => DontDestroyOnLoad(this);
    void Start() => client.client.Connect(ipAd, 25565);
    void Update() => HandleReadPackets();
    void OnEnable() => Register(ReceivePacket);
    void OnDisable() => Unregister(ReceivePacket);
    public void Register(ClientReader reader) => this.reader += reader;
    public void Unregister(ClientReader reader) => this.reader -= reader;


}
