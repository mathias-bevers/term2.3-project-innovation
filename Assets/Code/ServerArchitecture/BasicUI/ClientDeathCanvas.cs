using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientDeathCanvas : NetworkingBehaviour
{
    [SerializeField] GameObject backgroundPanel;
    [SerializeField] Text deathText;
    
    [SerializeField] List<DeathToText> deathToTexts = new List<DeathToText>();

    internal override void Awoken()
    {
        backgroundPanel.SetActive(false);
    }

    [NetworkRegistry(typeof(DeathEvent), TrafficDirection.Received)]
    public void Receive(ServerClient client, DeathEvent e, TrafficDirection direction)
    {
        if (e.ID != FindObjectOfType<UserClient>().ID) return;
        backgroundPanel.SetActive(true);
        foreach (var item in deathToTexts)
        {
            if (item.type != e.deathType) continue;
            deathText.text = item.Text;
        }
    }
}
