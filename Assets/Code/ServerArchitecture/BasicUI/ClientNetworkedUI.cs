using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [NetworkRegistry(typeof(BackToLobby), TrafficDirection.Received)]
    public void Receive(ServerClient client, BackToLobby backToLobby, TrafficDirection direction)
    {
        SceneManager.LoadScene("Lobby");
    }
}
