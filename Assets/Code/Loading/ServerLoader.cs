using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerLoader : BaseLoader
{
    List<ServerClient> servers = new List<ServerClient>();
    List<ServerClient> servers2 = new List<ServerClient>();


    bool STOP = false;
    [NetworkRegistry(typeof(SceneHasLoaded), TrafficDirection.Received)]
    public void Register(ServerClient client, SceneHasLoaded sceneHasLoaded, TrafficDirection direction)
    {
        if (sceneHasLoaded.scene != SceneManager.GetActiveScene().name) return;
        if (!servers.Contains(client))
            servers.Add(client);

        if (servers.Count != Settings.maxPlayerCount) return;

        SendMessage(new LoadSceneNow());
        LoadLevel();
        Debug.Log("Server Has Loaded: " + hasLoaded);

        if (!hasLoaded) return;
        if (servers2.Count != Settings.maxPlayerCount) return;
        SendMessage(new ReleaseScene());
        servers.Clear();
        servers2.Clear();
        Release();
        STOP = true;
    }

    [NetworkRegistry(typeof(LoadSceneNow), TrafficDirection.Received)]
    public void Register(ServerClient client, LoadSceneNow loadScene, TrafficDirection direction)
    {
        if (!servers2.Contains(client)) servers2.Add(client);
    }

    float timer = 0;
    private void Update()
    {
        if (STOP) timer = 0;
        timer += Time.deltaTime;
        if (timer >= 1)
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
