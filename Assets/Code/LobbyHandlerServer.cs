using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyHandlerServer : BaseLobbyHandler
{
    internal override void Awoken() 
    {
        for (int i = 0; i < spawnPoints.Length; i++)
            freePoints.Add(i);
    }

    [NetworkRegistry(typeof(ReadyRequest), TrafficDirection.Send)]
    public void Receive(ServerClient client, ReadyRequest list, TrafficDirection direction)
    {
        Debug.Log("I send a ready request!");
    }

    [NetworkRegistry(typeof(RequestNameChange), TrafficDirection.Received)]
    public void Receive(ServerClient client, RequestNameChange nameChange, TrafficDirection direction)
    {
        foreach (LobbyCharacter chara in spawnedChars)
        {
            if (chara.ID == nameChange.ID)
                chara.SetName(nameChange.Name);
        }
    }
}
