using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientLoader : BaseLoader
{
    [NetworkRegistry(typeof(SceneHasLoaded), TrafficDirection.Received)]
    public void Register(ServerClient client, SceneHasLoaded sceneHasLoaded, TrafficDirection direction)
    {
        if (sceneHasLoaded.scene != SceneManager.GetActiveScene().name) return;
        SendMessage(sceneHasLoaded);
    }
}
