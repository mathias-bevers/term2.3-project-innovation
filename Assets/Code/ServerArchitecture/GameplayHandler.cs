using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayHandler : NetworkingBehaviour
{
    [SerializeField] MarshMallowMovement character;
    [SerializeField] PointList pointList;
    [SerializeField] Transform arenaMiddle;
    [SerializeField] CameraMover cameraMover;
    [SerializeField] BakingZones bakingZone;

    List<MarshMallowMovement> spawnedCharacters = new List<MarshMallowMovement>();
    public List<MarshMallowMovement> SpawnedCharacters => spawnedCharacters;

    WinWay winWay = WinWay.None;
    List<int> winnerIDs = new List<int>();
    bool endingAchieved = false;
    float timer = 0;
    bool hasSpawned = false;

    public void Start()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameplayScene"));
    }

    [NetworkRegistry(typeof(UserList), TrafficDirection.Send)]
    public void Receive(ServerClient client, UserList list,TrafficDirection direction)
    {
        if (hasSpawned) return;
        if (list.Count != Settings.maxPlayerCount) return;
        if (character == null) return;
        for (int i = 0; i < list.Count; i++)
        {
            MarshMallowMovement newCharacter = Instantiate(character);
            newCharacter.ID = list[i].ID;
            newCharacter.SetColour(list[i].Colour);
            newCharacter.transform.position = pointList[i].position;
            newCharacter.transform.LookAt(arenaMiddle.transform);
            spawnedCharacters.Add(newCharacter);
        }
        hasSpawned = true;
        cameraMover?.Register(spawnedCharacters);
    }

    private void Update()
    {
        if(!endingAchieved) Clock();
        else
        {
            timer = 0;
            MarshMallowMovement[] data = FindObjectsOfType<MarshMallowMovement>();
            foreach (MarshMallowMovement m in data)
                m.enabled = false;
            WinningHandler w = gameObject.AddComponent<WinningHandler>();
            w.Setup(winnerIDs, winWay);
            Destroy(this);
        }
    }

    void Clock()
    {
        timer += Time.deltaTime;
        if (!hasSpawned && timer >= 3)
        {
            SendMessage(FindObjectOfType<MainServer>().GetUserList());
            timer = 0;
        }
        if (hasSpawned && timer >= 1)
        {
            HandleAfterSecond();
            timer = 0;
        }
    }

    void HandleAfterSecond()
    {
        if (endingAchieved) return;
        HandleDeaths();
        HandleBaking();
    }

    void HandleBaking()
    {
        foreach (MarshMallowMovement player in spawnedCharacters)
        {
            BakingData data = bakingZone.GetBonusValue(player);
            if (!data.isDamageType)
                player.currentBurnedCounter += data.bakingMultiplier;
        }

        BakingPacket packet = new BakingPacket();
        packet.maxBake = Settings.bakingMax;
        BakingPacketData[] data2 = new BakingPacketData[spawnedCharacters.Count];
        for (int i = 0; i < data2.Length; i++)
        {
            data2[i] = new BakingPacketData(spawnedCharacters[i].allowedID, spawnedCharacters[i].currentBurnedCounter);
            if (data2[i].actualAmount >= Settings.bakingMax)
            {
                endingAchieved = true;
            }
        }
        for(int i = 0; i < data2.Length; i++)
        {
            int winCount = 0;
            if (data2[i].actualAmount >= Settings.bakingMax)
            {
                winnerIDs.Add(data2[i].ID);
                winCount++;
            }
            if (winCount == 1) winWay = WinWay.SoloWin;
            else winWay = WinWay.TieWin;
        }
        packet.bakingPackets = data2;
        SendMessage(packet);
    }
    
    void HandleDeaths()
    {
        for (int i = spawnedCharacters.Count - 1; i >= 0; i--)
        {
            if (spawnedCharacters[i].transform.position.y <= -1)
            {
                SendMessage(new DeathEvent(spawnedCharacters[i].allowedID, Vector3.Distance(spawnedCharacters[i].transform.position, transform.position) < 5 ? DeathType.Burned : DeathType.Dirty));
                Destroy(spawnedCharacters[i].gameObject);
                spawnedCharacters.RemoveAt(i);
            }
        }

        if (spawnedCharacters.Count <= 1)
        {
            endingAchieved = true;
            if (spawnedCharacters.Count == 0) winWay = WinWay.TieLose;
            if (spawnedCharacters.Count == 1) winWay = WinWay.SoloWin;
            for(int i = 0; i < spawnedCharacters.Count; i++)
            {
                winnerIDs.Add(spawnedCharacters[i].allowedID);
            }
        }
    }

#if DEBUG
    int currentSelected = -1;

    public void OnGUI()
    {
        GUIUtility.ScaleAroundPivot(new Vector2(3, 3), Vector2.zero);
        GUI.Box(new Rect(10, 10, 30, 30), currentSelected.ToString());
        for(int i = 0; i < Settings.maxPlayerCount; i++)
        {
            if(GUI.Button(new Rect(10, 42 + (i * 32), 30, 30), new GUIContent(i.ToString(), "Select client number: " + i.ToString())))
            {
                if (i == currentSelected) currentSelected = -1;
                else currentSelected = i;
                FindObjectOfType<FakeClient>()?.SetID(currentSelected);
            }
        }
    }
#endif
}

public enum WinWay
{
    None,
    AllLose,
    TieLose,
    TieWin,
    SoloWin
}
