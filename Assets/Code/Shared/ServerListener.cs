using shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using static PacketHandler;

public class ServerListener : TcpListener, PacketHandler, IRegistrable
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

    public Dictionary<Type, Action<ServerClient, ISerializable>> callbacks { get; set; } = new Dictionary<Type, Action<ServerClient, ISerializable>>();

    public int ID => -1;

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

   UserList GenerateUserList()
    {
        UserList list = new UserList();
        List<DeclareUser> users = new List<DeclareUser>();
        foreach (ServerClient client in _clients)
            users.Add(client.self);
        list.users = users.ToArray();
        return list;
    }

    void OnPlayerCountChange()
    {
        foreach(ServerClient client in Clients)
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
        foreach (ServerClient client in clients)
        {
            try
            {
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
        for(int i = _clients.Count -1; i >= 0; i--) 
        {
            ServerClient client = _clients[i];
            try
            {
                if (client.client.Available == 0) continue;
                
                byte[] gottenBytes = StreamUtil.Read(client.client.GetStream());
                Packet packet = new Packet(gottenBytes);
                ISerializable current = packet.Read<ISerializable>();
                bool earlyCatch = EarlyCatch(client, current);

                Type storedType = current.GetType();
                if (callbacks.ContainsKey(storedType))
                    callbacks[storedType]?.Invoke(client, current);
                else if (!earlyCatch) reader?.Invoke(client, current);
            }
            catch(Exception e) { Debug.LogError(e); }
        }
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
