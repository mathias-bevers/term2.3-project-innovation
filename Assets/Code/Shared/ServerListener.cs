using shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEditor.PackageManager;
using UnityEngine;

public class ServerListener : TcpListener
{
    public ServerListener(IPAddress localaddr, int port) : base(localaddr, port) { }

    bool _isRunning = false;

    public bool IsRunning { get { return _isRunning; } }

    /// <summary>
    /// Will give a callback for all undeclared methods
    /// </summary>
    /// <param name="client">The client that send the packet</param>
    /// <param name="serialized">The serialized object that came from the packet</param>
    public delegate void ClientReader(TcpClient client, ISerializable serialized);
    ClientReader reader;

    List<TcpClient> _clients = new List<TcpClient>();

   public List<TcpClient> Clients { get { return _clients; } }

    public new void Start()
    {
        _isRunning = true;
        base.Start();
    }

    public new void Stop()
    {
        _isRunning = false;
        base.Stop();
    }

    Dictionary<Type, Action<TcpClient, ISerializable>> callbacks = new Dictionary<Type, Action<TcpClient, ISerializable>>();

    public void Declare<T>(Action<TcpClient, ISerializable> callback) where T : ISerializable
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
        TcpClient curClient = AcceptTcpClient();
        _clients.Add(curClient);
    }

    void HandleClients()
    {
        foreach(TcpClient client in _clients)
        {
            if (client.Available == 0) continue;

            byte[] gottenBytes = StreamUtil.Read(client.GetStream());
            Packet packet = new Packet(gottenBytes);
            ISerializable current = packet.Read<ISerializable>();

            Type storedType = current.GetType();
            if (callbacks.ContainsKey(storedType))
                callbacks[storedType]?.Invoke(client, current);
            else reader?.Invoke(client, current);
        }
    }
}
