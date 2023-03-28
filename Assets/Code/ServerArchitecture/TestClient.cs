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
    private IPAddress ipAd = IPAddress.Parse("192.168.2.10");
    ServerClient client = new ServerClient(new TcpClient(), -1);

    public Dictionary<Type, Action<ServerClient, ISerializable>> callbacks { get; set; } = new Dictionary<Type, Action<ServerClient, ISerializable>>();
    public ClientReader reader { get; set; }

    string myName = string.Empty;

    [SerializeField] TempPlayer player;
    public Transform[] spawnPoints;

    List<TempPlayer> spawnedPlayers = new List<TempPlayer>();

    public void Start()
    {
        Declare<Heartbeat>(ReceiveHeartbeat);
        Declare<DeclareUser>(ReceivedDeclareUser);
        Declare<UserList>(ReceiveUserList);
        Declare<Disconnected>(ReceiveDisconnected);

        client.client.Connect(ipAd, 5555);
    }

    void OnEnable() => Register(ReceivePacket);
    void OnDisable() => Unregister(ReceivePacket);
    

    void ReceivePacket(ServerClient client, ISerializable serializable)
    {
        Debug.Log("Received a undeclared packet: " + serializable.GetType());
    }

    void ReceivedDeclareUser(ServerClient client, ISerializable serializable)
    {
        Debug.Log("Received a new Spawn User packet");
        DeclareUser spawnUser = (DeclareUser)serializable;
        client.ID = spawnUser.ID;
        myName = spawnUser.Name;
        //SpawnPlayer(spawnUser);
    }

    void ReceiveUserList(ServerClient client, ISerializable serializable)
    {
        UserList userList = (UserList)serializable;
        DeclareUser[] users = userList.users;

        for(int i = 0; i < users.Length; i++)
        {
            SpawnPlayer(users[i]);
        }
    }

    void SpawnPlayer(DeclareUser forUser)
    {
        if (ContainsPlayer(forUser)) return;
        TempPlayer newPlayer = Instantiate(player);

        newPlayer.SetID(forUser.ID);
        newPlayer.SetName(forUser.Name);
        {
            newPlayer.transform.position = spawnPoints[spawnedPlayers.Count].position;
        }
        newPlayer.transform.parent = transform;
        spawnedPlayers.Add(newPlayer);
    }

    bool ContainsPlayer(DeclareUser user)
    {
        foreach(TempPlayer player in spawnedPlayers)
        {
            if (player.ID == user.ID) return true;
        }
        return false;
    }

    void ReceiveDisconnected(ServerClient client, ISerializable serializable)
    {
        Disconnected disconnected = (Disconnected)serializable;
        Debug.Log("Handle User Disconnection");

        for(int i = spawnedPlayers.Count - 1; i >= 0; i--)
        {
            if (spawnedPlayers[i].ID == disconnected.disconnectedID)
            {
                Destroy(spawnedPlayers[i].gameObject);
                spawnedPlayers.Remove(spawnedPlayers[i]);
            }
        }
    }

    private void Update()
    {
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


    void ReceiveHeartbeat(ServerClient client, ISerializable serializable) => SendPacket(serializable);
}
