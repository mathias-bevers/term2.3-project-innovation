using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerNetworkUI : BaseNetworkedUI
{
    [NetworkRegistry(typeof(DeathEvent), TrafficDirection.Send)]
    public void Receive(ServerClient client, DeathEvent e, TrafficDirection direction)
    {
        base.DeathReceived(e);
    }

    [NetworkRegistry(typeof(UserList), TrafficDirection.Send)]
    public void Receive(ServerClient client, UserList e, TrafficDirection direction)
    {
        base.OnUserList(e);
    }

}
