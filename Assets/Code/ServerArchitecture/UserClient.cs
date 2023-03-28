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

    private IPAddress ipAd = IPAddress.Parse("192.168.2.10");
    private ServerClient client = new ServerClient(new TcpClient(), -1);

    UserData currentData;
    public UserData CurrentData { get => currentData; }

    List<UserData> allClients = new List<UserData>();

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
        if (serializable is DeclareUser) 
        { 
            DeclareUser du = (DeclareUser)serializable; 
            currentData = new UserData(du.ID, du.Name, du.Colour); 
        }
        if(serializable is UserList)
        {
            UserList ul = (UserList)serializable;
        }
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
    void Start() => client.client.Connect(ipAd, 5555);
    void Update() => HandleReadPackets();
    void OnEnable() => Register(ReceivePacket);
    void OnDisable() => Unregister(ReceivePacket);
    public void Register(ClientReader reader) => this.reader += reader;
    public void Unregister(ClientReader reader) => this.reader -= reader;


}
