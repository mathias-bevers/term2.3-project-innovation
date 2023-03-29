using shared;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

public class MainServer : MonoBehaviour
{
    ServerListener server;

    ServerStates serverStates = ServerStates.Lobby;

    void Awake()
    {
        server.Declare<Score>(HandleScore);
        server.Declare<RequestNameChange>(HandleNameChange);
        server.Declare<Disconnected>(HandleDisconnect);
        server.Declare<ReadyRequest>(HandleReady);
        StartServer();
    }

    void HandleDisconnect(ServerClient client, ISerializable serializable)
    {
        server.DisconnectClient(client);
    }

    void HandleClient(ServerClient client, ISerializable serializable)
    {
        Debug.Log("Other Packet: " + serializable.GetType());
    }

    void HandleScore(ServerClient client, ISerializable serializable)
    {
        Score score = (Score)serializable;
        Debug.Log("Score: " + score.name + " : " + score.score);
    }

    void HandleReady(ServerClient client, ISerializable readyRequest)
    {
        ReadyRequest ready = (ReadyRequest)readyRequest;
        foreach(ServerClient client2 in server.Clients)
            if(client2.ID == client.ID)
                client2.isReady = ready.Readied;
        server.SendMessages(server.Clients, new ReadyRequest(client.ID, ready.Readied));
    }

    void HandleNameChange(ServerClient client, ISerializable serializable)
    {
        RequestNameChange nameChange = (RequestNameChange)serializable;
        string name = nameChange.Name;
        if (name.Length <= 3) return;
        if (name.Length > 12) return;
        foreach(ServerClient cli in server.Clients)
            if (cli.self.Name.ToLower() == name.ToLower()) 
                return;

        client.self.Name = nameChange.Name;
        server.SendMessages(server.Clients, new RequestNameChange(client.ID, nameChange.Name));
    }

    void DoUpdate()
    {
        server.Update();

        if (serverStates == ServerStates.Lobby) InLobby();
        if (serverStates == ServerStates.Loading) Debug.Log("Handle Loading");

    }


    void InLobby()
    {
        Debug.Log("Lobby gaming!");
        bool allReady = true;
        if (server.Clients.Count != Settings.maxPlayerCount) return;
        foreach (ServerClient client in server.Clients)
        {
            Debug.Log("Client: " + client.ID + ": " + client.isReady);
            if (client.isReady == false)
                allReady = false;
        }
        if (!allReady) return;

        serverStates = ServerStates.Loading;
        server.SendMessages(server.Clients, new ForceLoading());
    }

    public void StartServer() => server.Start();
    public void StopServer() => server.Stop();
    public MainServer() => server = new ServerListener(Settings.ip, Settings.port, Settings.maxPlayerCount);
    void OnEnable() =>      server.Register(HandleClient);
    void OnDisable() =>     server.Unregister(HandleClient);
    void Update() =>        DoUpdate();
    void FixedUpdate() =>   server.FixedUpdate(); 
}


enum ServerStates
{
    Lobby,
    Loading,
    Playing
}
