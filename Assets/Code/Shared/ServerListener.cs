using shared;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using static PacketHandler;

public class ServerListener : TcpListener, PacketHandler
{
    public ServerListener(IPAddress localaddr, int port, int maxPlayerCount) : base(localaddr, port) { this.maxPlayerCount = maxPlayerCount; }

    readonly int maxPlayerCount = 4;
    bool _isRunning = false;

    int currentServerCount = 0;

    public bool IsRunning { get { return _isRunning; } }

    public ClientReader reader { get; set; }

    List<ServerClient> _clients = new List<ServerClient>();

    public List<ServerClient> Clients { get { return _clients; } }

    public new void Start()
    {
        _isRunning = true;
        base.Start();
    }

    public new void Stop()
    {
        currentServerCount = 0;
        _isRunning = false;
        base.Stop();
    }

    public Dictionary<Type, NetworkedCallback> callbacks { get; set; } = new Dictionary<Type, NetworkedCallback>();

    public int ID => -1;

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

    public void Update()
    {
        if (!_isRunning) return;
        HandleNewClients();
        HandleClients();
        RidDeadClients();
    }

    public void FixedUpdate()
    {

    }

    public void SecondUpdate()
    {
        foreach (ServerClient client in Clients)
        {
            if (client.missedHeartbeats >= 3) client.markAsDead = true;
            else client.missedHeartbeats++;
        }
        SendMessages(Clients, new Heartbeat(50));
    }

    void RidDeadClients()
    {
        for (int i = Clients.Count - 1; i >= 0; i--)
        {
            ServerClient cur = Clients[i];
            if (!cur.markAsDead) continue;
            DisconnectClient(cur);
        }
    }

    public void DisconnectClient(ServerClient client)
    {
        Clients.Remove(client);
        SendMessages(Clients, new Disconnected(client.ID));
        SendMessages(Clients, GenerateUserList());
        OnPlayerCountChange();
    }

    void HandleNewClients()
    {
        if (!Pending()) return;
        if (Clients.Count >= maxPlayerCount) return;
        ServerClient curClient = new ServerClient(AcceptTcpClient(), currentServerCount);
        _clients.Add(curClient);
        SendMessage(curClient, curClient.self);
        SendMessages(_clients, GenerateUserList());
        currentServerCount++;
        OnPlayerCountChange();
    }

    public UserList GenerateUserList()
    {
        UserList list = new UserList();
        List<DeclareUser> users = new List<DeclareUser>();
        int c = 0;
        foreach (ServerClient client in _clients)
        {
            DeclareUser user = client.self;
            user.Colour = (ColourType)c;
            users.Add(user);
            c++;
        }
        list.users = users.ToArray();
        return list;
    }

    void OnPlayerCountChange()
    {
        foreach (ServerClient client in Clients)
        {
            client.isReady = false;
            SendMessages(Clients, new ReadyRequest(client.ID, false));
        }
    }

    internal void SendMessage(ServerClient client, ISerializable message) => SendMessages(new ServerClient[] { client }, message);
    internal void SendMessages(List<ServerClient> clients, ISerializable message) => SendMessages(clients.ToArray(), message);
    internal void SendMessages(ServerClient[] clients, ISerializable message)
    {
        Packet sendPacket = Convert(message);
        byte[] packetBytes = sendPacket.GetBytes();
        if (clients.Length == 0) reader?.Invoke(null, message, TrafficDirection.Send);
        foreach (ServerClient client in clients)
        {
            try
            {
                reader?.Invoke(client, message, TrafficDirection.Send);
                StreamUtil.Write(client.stream, packetBytes);
            }
            catch { }
        }
    }

    Packet Convert(ISerializable serializable)
    {
        Packet packet = new Packet();
        packet.Write(serializable);
        return packet;
    }

    void HandleClients()
    {
        for (int i = _clients.Count - 1; i >= 0; i--)
        {
            ServerClient client = _clients[i];
            try
            {
                if (client.client.Available == 0) continue;

                byte[] gottenBytes = StreamUtil.Read(client.client.GetStream());
                Packet packet = new Packet(gottenBytes);
                ISerializable current = packet.Read<ISerializable>();
                ReceivedPacket(client, current);

            }
            catch (Exception e) { Debug.LogError(e); }
        }
    }

    public ServerClient GetClientByID(int id)
    {
       foreach(ServerClient client in _clients)
        {
            if (client.ID == id) return client;
        }
        return null;
    }

    public void ReceivedPacket(ServerClient client, ISerializable current)
    {
        bool earlyCatch = EarlyCatch(client, current);

        Type storedType = current.GetType();
        if (!earlyCatch) reader?.Invoke(client, current, TrafficDirection.Received);
        if (callbacks.ContainsKey(storedType))
            callbacks[storedType]?.Invoke(client, current, TrafficDirection.Received);
    }

    bool EarlyCatch(ServerClient client, ISerializable serializable)
    {
        if (serializable is Heartbeat)
        {
            Heartbeat hb = serializable as Heartbeat;
            if (hb.num == 50) client.missedHeartbeats = 0;
            return true;
        }

        return false;
    }

    public void SendPacket(ISerializable serializable)
    {
        SendMessages(Clients, serializable);
    }
}
