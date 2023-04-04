using shared;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyHandler : BaseLobbyHandler
{
    [SerializeField] InputField inputField;

    [SerializeField] Button readyButton;
    [SerializeField] Button backButton;
    [SerializeField] Text readyText;

    [SerializeField] GameObject userCanvas;


    int lobbyPlayerCount = 0;

    internal override void Awoken()
    {
        for(int i = 0; i < spawnPoints.Length -1; i++)
            freePoints.Add(i);
    }

   
    [NetworkRegistry(typeof(DeclareUser), TrafficDirection.Received)]
    public void Receive(ServerClient client, DeclareUser user, TrafficDirection direction)
    {
        if (overrideClient is UserClient)
        {
            UserClient userClient = overrideClient as UserClient;
            self = userClient.CurrentData;
        }
        inputField.SetTextWithoutNotify(self.Name);
        userCanvas?.SetActive(true);

        if (Settings.preferName != string.Empty)
            SendMessage(new RequestNameChange(-1, Settings.preferName));
    }
    
    [NetworkRegistry(typeof(UserList), TrafficDirection.Received)]
    public void Receive(ServerClient client, UserList list, TrafficDirection direction)
    {
        lobbyPlayerCount = list.users.Length;
        bool lobbyAtMax = lobbyPlayerCount == Settings.maxPlayerCount;
        readyButton.gameObject.SetActive(lobbyAtMax);
#if DEBUG
        if (lobbyAtMax) SendMessage(new ReadyRequest(ID, true));
#endif
    }
    
    [NetworkRegistry(typeof(ReadyRequest), TrafficDirection.Received)]
    public void Receive(ServerClient client, ReadyRequest readyRequest, TrafficDirection direction)
    {
        foreach (LobbyCharacter chara in spawnedChars)
        {
            if (self.ID == chara.ID)
                readyText.text = (chara.ready ? "Ready [X]" : "Ready [✓]");
        }
    }

    [NetworkRegistry(typeof(ForceLoading), TrafficDirection.Received)]
    public void Receive(ServerClient client, ForceLoading loading, TrafficDirection direction)
    {
        userCanvas?.SetActive(false);
    }

    public void SendNameRequest(string name)
    {
        SendMessage(new RequestNameChange(ID, name));
    }

    public void BackPressed()
    {
        if (overrideClient is UserClient)
        {
            UserClient userClient = overrideClient as UserClient;
            Destroy(userClient.gameObject);
        }
        SceneManager.LoadScene("IPScene");
    }

    public void ReadyPressed()
    {
        self.ready = !self.ready;
        SendMessage(new ReadyRequest(ID, self.ready));
    }
}
