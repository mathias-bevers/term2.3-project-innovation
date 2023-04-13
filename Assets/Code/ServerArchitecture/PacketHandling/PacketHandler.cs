using shared;
using System;
using System.Collections.Generic;
using static ServerListener;

public interface PacketHandler
{
    void Register(ClientReader reader);
    void Unregister(ClientReader reader);
    void Declare<T>(NetworkedCallback callback) where T : ISerializable;
    Dictionary<Type, NetworkedCallback> callbacks { get; set; }
    ClientReader reader { get; set; }

    /// <summary>
    /// Will give a callback for all undeclared methods
    /// </summary>
    /// <param name="client">The client that send the packet</param>
    /// <param name="serialized">The serialized object that came from the packet</param>
    public delegate void ClientReader(ServerClient client, ISerializable serialized, TrafficDirection trafficDirection);
}

[System.Flags]
public enum TrafficDirection
{
    Received,
    Send,
    Both
}

public enum NetworkTarget
{
    Client,
    Server,
    FakeClient,
    None
}
