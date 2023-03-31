using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyHandlerServer : NetworkingBehaviour
{
    [Space(10)]
    [SerializeField] LobbyCharacter lobbyCharacterPrefab;

    [SerializeField]
    Transform[] spawnPoints;

    List<int> freePoints = new List<int>();
    List<LobbyCharacter> spawnedChars = new List<LobbyCharacter>();

    private void Start()
    {
        for(int i = 0; i < spawnPoints.Length; i++)
            freePoints.Add(i);
    }

    [NetworkRegistry(typeof(UserList), TrafficDirection.Send)]
    void Receive(ServerClient client, UserList list, TrafficDirection direction)
    {
        Debug.Log("I send a user list!");
    }

    [NetworkRegistry(typeof(ReadyRequest), TrafficDirection.Send)]
    void Receive(ServerClient client, ReadyRequest list, TrafficDirection direction)
    {
        Debug.Log("I send a ready request!");
    }
}
