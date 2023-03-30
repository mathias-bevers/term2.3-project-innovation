using shared;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class MainServer : MonoBehaviour, IRegistrable
{
    ServerListener server;

    ServerStates serverStates = ServerStates.Lobby;

    void Awake()
    {
        server.Declare<Score>(HandleScore);
        server.Declare<RequestNameChange>(HandleNameChange);
        server.Declare<Disconnected>(HandleDisconnect);
        server.Declare<ReadyRequest>(HandleReady);
        server.Declare<AskLoadingEntered>(HandleLoadingEntered);
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

    void HandleLoadingEntered(ServerClient client, ISerializable serializable)
    {
        AskLoadingEntered hasLoaded = (AskLoadingEntered)serializable;
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
        else if (serverStates == ServerStates.Loading) { }
    }

    void DoSecondUpdate()
    {
        server.SecondUpdate();
        if (serverStates == ServerStates.Lobby) { }
        else if (serverStates == ServerStates.Loading) 
        {
            server.SendMessages(server.Clients, new AskLoadingEntered());
        }
    }

    float counter = 0;

    public PacketHandler.ClientReader reader { get => server.reader; set => server.reader = value; }

    public int ID => server.ID;

    void DoFixedUpdate()
    {
        server.FixedUpdate();
        counter += Time.fixedUnscaledDeltaTime;
        if (counter >= 1)
        {
            counter = 0;
            DoSecondUpdate();
        }
    }

    void InLobby()
    {
        bool allReady = true;
        if (server.Clients.Count != Settings.maxPlayerCount) return;
        foreach (ServerClient client in server.Clients)
            if (!client.isReady)
                allReady = false;
        if (!allReady) return;

        serverStates = ServerStates.Loading;
        server.SendMessages(server.Clients, new ForceLoading());
    }

    public void StartServer() => server.Start();
    public void StopServer() => server.Stop();
    public MainServer() =>  server = new ServerListener(Settings.serverIP, Settings.port, Settings.maxPlayerCount);
    void OnEnable() =>      server.Register(HandleClient);
    void OnDisable() =>     server.Unregister(HandleClient);
    void Update() =>        DoUpdate();
    void FixedUpdate() =>   DoFixedUpdate();

    public void Register(PacketHandler.ClientReader reader)
    {
        server.Register(reader);
    }

    public void Unregister(PacketHandler.ClientReader reader)
    {
        server.Unregister(reader);
    }

    public void SendPacket(ISerializable serializable)
    {
       server.SendPacket(serializable);
    }
}


enum ServerStates
{
    Lobby,
    Loading,
    Playing
}
