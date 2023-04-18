using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinningHandler : NetworkingBehaviour
{
    List<int> winningIds;
    WinWay winWay;

    bool startup = false;

    public void Setup(List<int> winningIds, WinWay winWay)
    {
        this.winningIds = winningIds;
        this.winWay = winWay;
        startup = true;
        
    }

    float timer = 0;
    bool hasSend = false;
    bool hasSend2 = false;
    
    private void Update()
    {
        if (!startup) return;

        timer += Time.deltaTime;
        if (timer >= 3)
        {
            if (!hasSend)
            {
                SendMessage(new WinnerPacket(winningIds, winWay));
                hasSend = true;
            }
        }
        if (timer >= 6)
        {
            if (!hasSend2)
            {
                SendMessage(new BackToLobby());
                hasSend2 = true;
                
            }
        }
    }

    [NetworkRegistry(typeof(BackToLobby), TrafficDirection.Send)]
    public void Receive(ServerClient client, BackToLobby backToLobby, TrafficDirection direction)
    {
        ((MainServer)overrideClient).StopServer();
        Destroy(overrideClient.gameObject);
        SceneManager.LoadScene("Lobby");
    }
}
