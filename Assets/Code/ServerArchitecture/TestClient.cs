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
    ServerClient client = new ServerClient(new TcpClient(), -1);

    public Dictionary<Type, NetworkedCallback> callbacks { get; set; } = new Dictionary<Type, NetworkedCallback>();
    public ClientReader reader { get; set; }

    string myName = string.Empty;

    [SerializeField] TempPlayer player;
    public Transform[] spawnPoints;

    List<TempPlayer> spawnedPlayers = new List<TempPlayer>();

    public void Start()
    {
        Declare<Heartbeat>(new NetworkedCallback(ReceiveHeartbeat, TrafficDirection.Received));
        Declare<DeclareUser>(new NetworkedCallback(ReceivedDeclareUser, TrafficDirection.Received));
        Declare<UserList>(new NetworkedCallback(ReceiveUserList, TrafficDirection.Received));
        Declare<Disconnected>(new NetworkedCallback(ReceiveDisconnected, TrafficDirection.Received));

        client.client.Connect(Settings.ip, Settings.port);
    }

    void OnEnable() => Register(ReceivePacket);
    void OnDisable() => Unregister(ReceivePacket);
    

    void ReceivePacket(ServerClient client, ISerializable serializable, TrafficDirection trafficDirection)
    {
        if (trafficDirection != TrafficDirection.Received) return;
        Debug.Log("Received a undeclared packet: " + serializable.GetType());
    }

    void ReceivedDeclareUser(ServerClient client, ISerializable serializable, TrafficDirection direction)
    {
        Debug.Log("Received a new Spawn User packet");
        DeclareUser spawnUser = (DeclareUser)serializable;
        client.ID = spawnUser.ID;
        myName = spawnUser.Name;
        //SpawnPlayer(spawnUser);
    }

    void ReceiveUserList(ServerClient client, ISerializable serializable, TrafficDirection direction)
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

    void ReceiveDisconnected(ServerClient client, ISerializable serializable, TrafficDirection direction)
    {
        Disconnected disconnected = (Disconnected)serializable;

        for(int i = spawnedPlayers.Count - 1; i >= 0; i--)
        {
            if (spawnedPlayers[i].ID == disconnected.ID)
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
            callbacks[storedType]?.Invoke(client, current, TrafficDirection.Received);
        else reader?.Invoke(client, current, TrafficDirection.Received);
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

    public void Declare<T>(NetworkedCallback callback) where T : ISerializable
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


    void ReceiveHeartbeat(ServerClient client, ISerializable serializable, TrafficDirection direction) => SendPacket(serializable);
}
