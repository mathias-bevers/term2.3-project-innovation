using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerNetworkUI : BaseNetworkedUI
{
    [NetworkRegistry(typeof(DeathEvent), TrafficDirection.Send)]
    public void Receive(ServerClient client, DeathEvent e, TrafficDirection direction)
    {
        DeathReceived(e);
    }

    [NetworkRegistry(typeof(UserList), TrafficDirection.Send)]
    public void Receive(ServerClient client, UserList e, TrafficDirection direction)
    {
        OnUserList(e);
    }

    [NetworkRegistry(typeof(BakingPacket), TrafficDirection.Send)]
    public void Receive(ServerClient client, BakingPacket e, TrafficDirection direction)
    {
        ReceiveBakingPacket(e);
    }
}
