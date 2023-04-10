using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientWinScreen : WinScreen
{
    [NetworkRegistry(typeof(WinnerPacket), TrafficDirection.Received)]
    public void Receive(ServerClient client, WinnerPacket packet, TrafficDirection direction)
    {
        OnWinnerPacket(packet);
    }
}
