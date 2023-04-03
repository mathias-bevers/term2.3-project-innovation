using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayHandler : NetworkingBehaviour
{
    [SerializeField] GameplayCharacter character;
    [SerializeField] PointList pointList;
    [SerializeField] Transform arenaMiddle;

    List<GameplayCharacter> spawnedCharacters = new List<GameplayCharacter>();

    [NetworkRegistry(typeof(UserList), TrafficDirection.Send)]
    public void Receive(ServerClient client, UserList list, TrafficDirection direction)
    {
        if (character == null) return;
        for (int i = 0; i < pointList.Count; i++)
        {
            GameplayCharacter newCharacter = Instantiate(character);
            spawnedCharacters.Add(newCharacter);
            newCharacter.transform.position = pointList[i].position;
            newCharacter.transform.LookAt(arenaMiddle.transform);
        }
    }
}
