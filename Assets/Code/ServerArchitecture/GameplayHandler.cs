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

    List<MarshMallowMovement> spawnedCharacters = new List<MarshMallowMovement>();



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
            Debug.Log(newCharacter.ID);
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
        timer += Time.deltaTime;
        if(!hasSpawned && timer >= 3)
        {
            SendMessage(FindObjectOfType<MainServer>().GetUserList());
        }
        for(int i = spawnedCharacters.Count - 1; i >= 0; i--)
        {
            if (spawnedCharacters[i].transform.position.y <= -1) 
            {
                SendMessage(new DeathEvent(spawnedCharacters[i].allowedID, DeathType.Dirty));
                Destroy(spawnedCharacters[i].gameObject);
                spawnedCharacters.RemoveAt(i);
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
