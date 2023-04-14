using shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using static PacketHandler;
using UnityEngine.SceneManagement;
using System.Threading;

public class UserClient : IRegistrable
{
    public override ClientReader reader { get; set; }
    public override int ID => client.ID;

    private ServerClient client = new ServerClient(new TcpClient(), -1);
    public ServerClient Client { get => client; }

    UserData currentData;
    public UserData CurrentData { get => currentData; }

    float timerWithoutHeartbeat = 0;
    int penaltyCount = 0;

    List<ISerializable> serializables = new List<ISerializable>();

    void HandleReadPackets()
    {
        if (client == null) return;
        timerWithoutHeartbeat += Time.deltaTime;
        if(timerWithoutHeartbeat >= 3.5f || penaltyCount >= 5)
        {
            SendPacket(new Disconnected());
            SceneManager.LoadScene("MainMenu");
            Destroy(this);
            return;
        }
        if (client.Available == 0) return;


        //new Thread(() =>
        //{
            byte[] gottenBytes = StreamUtil.Read(client.stream);
            Packet packet = new Packet(gottenBytes);
            ISerializable current = packet.Read<ISerializable>();
            reader?.Invoke(client, current, TrafficDirection.Received);
        //serializables.Add(current);
        //}).Start();

        for (int i = serializables.Count - 1; i >= 0; i--)
        {
            //reader?.Invoke(client, serializables[i], TrafficDirection.Received);
            //serializables.RemoveAt(i);
        }
    }

    void ReceivePacket(ServerClient client, ISerializable serializable, TrafficDirection trafficDirection)
    {
        if (trafficDirection != TrafficDirection.Received) return;
        if (serializable is Heartbeat) { timerWithoutHeartbeat = 0; SendPacket(serializable); }
        if (serializable is DeclareUser) 
        { 
            DeclareUser du = (DeclareUser)serializable; 
            currentData = new UserData(du.ID, du.Name, du.Colour);
            client.ID = du.ID;
        }
    }

    public override void SendPacket(ISerializable serializable)
    {
        Packet packet = new Packet();
        packet.Write(serializable);
        try {
            reader?.Invoke(client, serializable, TrafficDirection.Send);
            //new Thread(() => StreamUtil.Write(client.stream, packet.GetBytes())).Start();
            StreamUtil.Write(client.stream, packet.GetBytes());
        } catch(Exception e) { Debug.LogError(e); penaltyCount++; }
    }

    void Awake()
    {
        if (FindObjectsOfType<UserClient>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this); 
    }
    void Start() => client.client.Connect(new IPEndPoint(Settings.ip, Settings.port));
    void Update() => HandleReadPackets();
    void OnEnable() => Register(ReceivePacket);
    void OnDisable() => Unregister(ReceivePacket);
    void OnDestroy() { SendPacket(new Disconnected()); client.client.Close(); }
    public override void Register(ClientReader reader) => this.reader += reader;
    public override void Unregister(ClientReader reader) => this.reader -= reader;
}
