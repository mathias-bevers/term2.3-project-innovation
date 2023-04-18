using shared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedCallback 
{
    public Action<ServerClient, ISerializable, TrafficDirection> callback;
    public TrafficDirection direction;

    public NetworkedCallback(Action<ServerClient, ISerializable, TrafficDirection> callback, TrafficDirection direction)
    {
        this.callback = callback;
        this.direction = direction;
    }

    public void Invoke(ServerClient client, ISerializable current, TrafficDirection direction)
    {
        bool pass = false;
        if (this.direction == TrafficDirection.Both) pass = true;
        if (this.direction == direction) pass = true;
        if(pass) callback?.Invoke(client, current, direction);
    }
}
