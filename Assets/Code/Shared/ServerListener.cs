using shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEditor.PackageManager;
using UnityEngine;
using static PacketHandler;

public class ServerListener : TcpListener, PacketHandler
{
    public ServerListener(IPAddress localaddr, int port) : base(localaddr, port) { }

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

    public void Declare<T>(Action<ServerClient, ISerializable> callback) where T : ISerializable
    {
        if(!callbacks.ContainsKey(typeof(T)))
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
    }

    void HandleNewClients()
    {
        if (!Pending()) return;
        ServerClient curClient = new ServerClient(AcceptTcpClient(), currentServerCount);
        _clients.Add(curClient);
        SendMessage(curClient, new DeclareUser(currentServerCount, "User" + currentServerCount.ToString(), new Vector3(1, 1, 1)));
        currentServerCount++;
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
        foreach(ServerClient client in _clients)
        {
            try
            {
                if (client.client.Available == 0) continue;

                byte[] gottenBytes = StreamUtil.Read(client.client.GetStream());
                Packet packet = new Packet(gottenBytes);
                ISerializable current = packet.Read<ISerializable>();

                Type storedType = current.GetType();
                if (callbacks.ContainsKey(storedType))
                    callbacks[storedType]?.Invoke(client, current);
                else reader?.Invoke(client, current);
            }
            catch { }
        }
    }
}
