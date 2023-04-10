using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerWinScreen : WinScreen
{
    [NetworkRegistry(typeof(WinnerPacket), TrafficDirection.Send)]
    public void Receive(ServerClient client, WinnerPacket packet, TrafficDirection direction)
    {
        OnWinnerPacket(packet);
    }
}
