using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLobbyHandler : NetworkingBehaviour
{
    [Space(10)]
    [SerializeField] LobbyCharacter lobbyCharacterPrefab;

    [SerializeField]
    internal Transform[] spawnPoints;

    internal List<int> freePoints = new List<int>();
    internal List<LobbyCharacter> spawnedChars = new List<LobbyCharacter>();


    internal UserData self = new UserData(-1);

    [NetworkRegistry(typeof(UserList), TrafficDirection.Both)]
    public void BaseReceive(ServerClient client, UserList list, TrafficDirection direction)
    {
        foreach (DeclareUser item in list.users)
            SpawnCharacter(item);
    }

    [NetworkRegistry(typeof(ReadyRequest), TrafficDirection.Both)]
    public void BaseReceive(ServerClient client, ReadyRequest readyRequest, TrafficDirection direction)
    {
        foreach (LobbyCharacter chara in spawnedChars)
        {
            if (chara.ID == readyRequest.ID)
                chara.SetReady(readyRequest.Readied);
        }
    }

    [NetworkRegistry(typeof(Disconnected), TrafficDirection.Both)]
    public void BaseReceive(ServerClient client, Disconnected disconnected, TrafficDirection direction)
    {
        RemoveCharacter(disconnected.ID);
    }


    [NetworkRegistry(typeof(RequestNameChange), TrafficDirection.Both)]
    public void Receive(ServerClient client, RequestNameChange nameChange, TrafficDirection direction)
    {
        foreach (LobbyCharacter chara in spawnedChars)
        {
            if (chara.ID == nameChange.ID)
                chara.SetName(nameChange.Name);
        }
    }

    internal void SpawnCharacter(DeclareUser user)
    {
        if (HasSpawned(user)) return;

        LobbyCharacter character = Instantiate(lobbyCharacterPrefab);
        spawnedChars.Add(character);
        character.transform.parent = transform;
        character.SetID(user.ID);
        character.SetName(user.Name);
        if (user.ID == self.ID)
        {
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

    internal void RemoveCharacter(int ID)
    {
        if (!HasSpawned(ID)) return;
        for (int i = spawnedChars.Count - 1; i >= 0; i--)
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

}
