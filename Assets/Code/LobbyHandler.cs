using shared;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyHandler : NetworkingBehaviour
{
    [Space(10)]
    [SerializeField] LobbyCharacter lobbyCharacterPrefab;

    [SerializeField]
    Transform[] spawnPoints;
    List<int> freePoints = new List<int>();
    List<LobbyCharacter> spawnedChars = new List<LobbyCharacter>();

    UserData self;

    [SerializeField] InputField inputField;

    [SerializeField] Button readyButton;
    [SerializeField] Button backButton;
    [SerializeField] Text readyText;

    [SerializeField] GameObject userCanvas;
    [SerializeField] GameObject loadingCanvas;

    int lobbyPlayerCount = 0;

    internal override void Awoken()
    {
        for(int i = 0; i < spawnPoints.Length -1; i++)
            freePoints.Add(i);
    }

    [NetworkRegistry(typeof(DeclareUser))]
    public void Receive(ServerClient client, DeclareUser user)
    {
        self = overrideClient.CurrentData;
        inputField.SetTextWithoutNotify(self.Name);
        
    }
    
    [NetworkRegistry(typeof(UserList))]
    public void Receive(ServerClient client, UserList list)
    {
        lobbyPlayerCount = list.users.Length;
        bool lobbyAtMax = lobbyPlayerCount == Settings.maxPlayerCount;
        readyButton.gameObject.SetActive(lobbyAtMax);

        foreach (DeclareUser item in list.users)
            SpawnCharacter(item);
    }

    
    [NetworkRegistry(typeof(ReadyRequest))]
    public void Receive(ServerClient client, ReadyRequest readyRequest)
    {
        foreach (LobbyCharacter chara in spawnedChars)
        {
            if (chara.ID == readyRequest.ID)
                chara.SetReady(readyRequest.Readied);
            if (self.ID == chara.ID)
                readyText.text = (chara.ready ? "Ready [✓]" : "Ready [X]");
        }
    }

    [NetworkRegistry(typeof(Disconnected))]
    public void Receive(ServerClient client, Disconnected disconnected)
    {
        RemoveCharacter(disconnected.ID);
    }

    [NetworkRegistry(typeof(RequestNameChange))]
    public void Receive(ServerClient client, RequestNameChange nameChange)
    {
        foreach (LobbyCharacter chara in spawnedChars)
        {
            if (chara.ID == nameChange.ID)
                chara.SetName(nameChange.Name);
           
        }
    }

    [NetworkRegistry(typeof(ForceLoading))]
    public void Receive(ServerClient client, ForceLoading loading)
    {
        userCanvas?.SetActive(false);
        loadingCanvas?.SetActive(true);
    }

    void SpawnCharacter(DeclareUser user)
    {
        if(HasSpawned(user)) return;

        LobbyCharacter character = Instantiate(lobbyCharacterPrefab);
        spawnedChars.Add(character);
        character.transform.parent = transform;
        character.SetID(user.ID);
        character.SetName(user.Name);
        if (user.ID == self.ID) {
            character.assignedNum = spawnPoints.Length - 1;
            character.transform.position = spawnPoints[spawnPoints.Length - 1].position;
        }
        else
        {
            int gottenNum = Random.Range(0, freePoints.Count);
            character.transform.position = spawnPoints[freePoints[gottenNum]].position;
            character.assignedNum = freePoints[gottenNum];
            freePoints.RemoveAt(gottenNum);
        }
    }

    void RemoveCharacter(int ID)
    {
        if (!HasSpawned(ID)) return;
        for(int i = spawnedChars.Count -1; i >= 0; i--)
        {
            LobbyCharacter chara = spawnedChars[i];
            if (chara.ID != ID) continue;
            spawnedChars.Remove(chara);
            freePoints.Add(chara.assignedNum);
            Destroy(chara.gameObject);
        }
    }

    bool HasSpawned(DeclareUser user) => HasSpawned(user.ID);
    bool HasSpawned(int id)
    {
        foreach (LobbyCharacter chara in spawnedChars)
            if (chara.ID == id) return true;
        return false;
    }

    public void SendNameRequest(string name)
    {
        SendMessage(new RequestNameChange(ID, name));
    }

    public void BackPressed()
    {
        Destroy(overrideClient.gameObject);
        SceneManager.LoadScene("IPScene");
    }

    public void ReadyPressed()
    {
        self.ready = !self.ready;
        SendMessage(new ReadyRequest(ID, self.ready));
    }
}
