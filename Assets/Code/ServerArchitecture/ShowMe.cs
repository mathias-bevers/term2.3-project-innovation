using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowMe : NetworkingBehaviour
{
    [SerializeField] Image blackPanel;
    [NetworkRegistry(typeof(Heartbeat), TrafficDirection.Both)]
    public void Register(ServerClient client, Heartbeat heartbeat, TrafficDirection direction)
    {
        blackPanel.gameObject.SetActive(false);
    }
}
