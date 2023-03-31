using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerLoader : BaseLoader
{
    List<ServerClient> servers = new List<ServerClient>();

    int step = 0;

    [NetworkRegistry(typeof(SceneHasLoaded), TrafficDirection.Received)]
    public void Register(ServerClient client, SceneHasLoaded sceneHasLoaded, TrafficDirection direction)
    {
        if (sceneHasLoaded.scene != SceneManager.GetActiveScene().name) return;
        if (!servers.Contains(client)) 
            servers.Add(client);

        if (servers.Count == Settings.maxPlayerCount)
            Debug.Log("All scenes should've loaded!");
    }


    float timer = 0;
    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > 1)
        {
            timer = 0;
            OnSecond();
        }
    }

    void OnSecond()
    {
        
        SendMessage(new SceneHasLoaded(SceneManager.GetActiveScene().name));
    }
}

public enum LoadingStep
{
    InitialSceneLoadConfirmation,
    RequestNewSceneLoad,
    SecundairySceneLoadConfirmation
}
