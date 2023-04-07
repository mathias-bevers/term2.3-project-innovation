using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientNetworkedUI : BaseNetworkedUI
{
    [NetworkRegistry(typeof(DeathEvent), TrafficDirection.Received)]
    public void Receive(ServerClient client, DeathEvent e, TrafficDirection direction)
    {
        base.DeathReceived(e);
    }

    [NetworkRegistry(typeof(UserList), TrafficDirection.Received)]
    public void Receive(ServerClient client, UserList e, TrafficDirection direction)
    {
        base.OnUserList(e);
    }

    [NetworkRegistry(typeof(BakingPacket), TrafficDirection.Received)]
    public void Receive(ServerClient client, BakingPacket e, TrafficDirection direction)
    {
        base.ReceiveBakingPacket(e);
    }
}
