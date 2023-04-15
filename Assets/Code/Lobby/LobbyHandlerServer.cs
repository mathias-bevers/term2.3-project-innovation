using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyHandlerServer : BaseLobbyHandler
{

    [SerializeField] GameObject canvas;
    internal override void Awoken() 
    {
        for (int i = 0; i < spawnPoints.Length; i++)
            freePoints.Add(i);
        canvas?.SetActive(true);
    }
}
