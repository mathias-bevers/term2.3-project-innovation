using shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using static PacketHandler;

public class TestClient : MonoBehaviour, PacketHandler
{
    private IPAddress ipAd = IPAddress.Parse("127.0.0.1");
    ServerClient client = new ServerClient(new TcpClient(), -1);

    public Dictionary<Type, Action<ServerClient, ISerializable>> callbacks { get; set; } = new Dictionary<Type, Action<ServerClient, ISerializable>>();
    public ClientReader reader { get; set; }

    string myName = string.Empty;


    public void Start()
    {
        Declare<DeclareUser>(ReceivedSpawnUser);
    }

    void OnEnable() => Register(ReceivePacket);
    void OnDisable() => Unregister(ReceivePacket);
    

    void ReceivePacket(ServerClient client, ISerializable serializable)
    {
        Debug.Log("Received a undeclared packet: " + serializable.GetType());
    }

    void ReceivedSpawnUser(ServerClient client, ISerializable serializable)
    {
        Debug.Log("Received a new Spawn User packet");
        DeclareUser spawnUser = (DeclareUser)serializable;
        client.ID = spawnUser.ID;
        myName = spawnUser.Name;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C)) client.client.Connect(ipAd, 25565);

        if (Input.GetKeyDown(KeyCode.Space))
            SendPacket(new shared.Score("testScore", 99));

        if (Input.GetKeyDown(KeyCode.G))
            SendPacket(new shared.AddRequest());

        if (Input.GetKeyDown(KeyCode.B))
            SendPacket(new shared.GetRequest());


        HandleReadPackets();
    }

    void HandleReadPackets()
    {
        if (client.Available == 0) return;

        byte[] gottenBytes = StreamUtil.Read(client.stream);
        Packet packet = new Packet(gottenBytes);
        ISerializable current = packet.Read<ISerializable>();

        Type storedType = current.GetType();
        if (callbacks.ContainsKey(storedType))
            callbacks[storedType]?.Invoke(client, current);
        else reader?.Invoke(client, current);
    }

    void SendPacket(ISerializable serializable)
    {
        Packet packet = new Packet();
        packet.Write(serializable);
        try
        {
            StreamUtil.Write(client.stream, packet.GetBytes());
        }
        catch { }
    }

    public void Declare<T>(Action<ServerClient, ISerializable> callback) where T : ISerializable
    {
        if (!callbacks.ContainsKey(typeof(T)))
            callbacks.Add(typeof(T), callback);
    }

    public void Register(ClientReader reader)
    {
        this.reader += reader;
    }

    public void Unregister(ClientReader reader)
    {
        this.reader -= reader;
    }

    void OnGUI()
    {
        GUI.matrix = Matrix4x4.Scale((Screen.width / 1080) * Vector3.one);
        GUI.Label(new Rect(transform.position.x * 720, transform.position.y * 720, 100, 100), client.ID.ToString() + " : " +  myName);
    }
}
