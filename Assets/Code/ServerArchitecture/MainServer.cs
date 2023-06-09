using shared;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PacketHandler;

public class MainServer : IRegistrable
{
    ServerListener server;

    public ServerListener Server { get => server; }

    ServerStates serverStates = ServerStates.Lobby;

    public void Awake()
    {
        if (FindObjectsOfType<MainServer>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
        server.Declare<RequestNameChange>(new NetworkedCallback(HandleNameChange, TrafficDirection.Received));
        server.Declare<Disconnected>(new NetworkedCallback(HandleDisconnect, TrafficDirection.Both));
        server.Declare<ReadyRequest>(new NetworkedCallback(HandleReady, TrafficDirection.Received));
        server.Declare<AskLoadingEntered>(new NetworkedCallback(HandleLoadingEntered, TrafficDirection.Send));
        server.Declare<BackToLobby>(new NetworkedCallback(HandleLobbyLoading, TrafficDirection.Send));
        server.Declare<ReleaseScene>(new NetworkedCallback(ReleasedScene, TrafficDirection.Send));
        StartServer();
    }

    bool hasQuit = false;

    public void HandleDisconnect(ServerClient client, ISerializable serializable, TrafficDirection direction)
    {
        if (serverStates == ServerStates.Lobby) server.DisconnectClient(client);
        else
        {
            foreach (ServerClient c in server.Clients)
            {
                SendPacket(new Disconnected(c.ID));
            }
            if (hasQuit) return;
            hasQuit = true;
            server.Stop();
            Destroy(gameObject);
            SceneManager.LoadScene("Lobby");
        }
    }

    void HandleClient(ServerClient client, ISerializable serializable, TrafficDirection direction)
    {

    }

    void ReleasedScene(ServerClient client, ISerializable serializable, TrafficDirection dir)
    {
        serverStates = ServerStates.Playing;
    } 

    void HandleLobbyLoading(ServerClient client, ISerializable serializable, TrafficDirection direction)
    {
        server.Stop();
        Destroy(gameObject);
    }

    void HandleLoadingEntered(ServerClient client, ISerializable serializable, TrafficDirection direction)
    {
        AskLoadingEntered hasLoaded = (AskLoadingEntered)serializable;
    }

    void HandleReady(ServerClient client, ISerializable readyRequest, TrafficDirection direction)
    {
        ReadyRequest ready = (ReadyRequest)readyRequest;
        foreach (ServerClient client2 in server.Clients)
            if (client2.ID == client.ID)
                client2.isReady = ready.Readied;
        server.SendMessages(server.Clients, new ReadyRequest(client.ID, ready.Readied));
    }

    void HandleNameChange(ServerClient client, ISerializable serializable, TrafficDirection direction)
    {
        RequestNameChange nameChange = (RequestNameChange)serializable;
        string name = nameChange.Name;
        if (name.Length < 3) return;
        if (name.Length > 12) return;
        foreach (ServerClient cli in server.Clients)
            if (cli.self.Name.ToLower() == name.ToLower())
                return;

        client.self.Name = nameChange.Name;
        server.SendMessages(server.Clients, new RequestNameChange(client.ID, nameChange.Name));
    }

    float waitTimer = 0;

    void DoUpdate()
    {
        server.Update();

        if(server.Clients.Count != Settings.maxPlayerCount && serverStates != ServerStates.Lobby)
        {
            HandleDisconnect(null, new Disconnected(), TrafficDirection.Received);
        }

        if (serverStates == ServerStates.Lobby) InLobby();
        else if (serverStates == ServerStates.Loading) { }
        else if (serverStates == ServerStates.Returning)
        {
            waitTimer += Time.deltaTime;
            if(waitTimer >= 4)
            {
                server.SendMessages(server.Clients, server.GenerateUserList());
                serverStates = ServerStates.Lobby;
                waitTimer = 0;
            }
        }
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

    public override ClientReader reader { get => server.reader; set => server.reader = value; }
    public override int ID => server.ID;

    void DoFixedUpdate()
    {
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

    public void StartServer() { server.Start();  }
    public void StopServer() => server.Stop();
    public MainServer() => server = new ServerListener(Settings.serverIP, Settings.port, Settings.maxPlayerCount);
    void OnEnable() => server.Register(HandleClient);
    void OnDisable() => server.Unregister(HandleClient);
    void Update() => DoUpdate();
    void FixedUpdate() => DoFixedUpdate();

    public override void Register(PacketHandler.ClientReader reader)
    {
        server.Register(reader);
    }

    public override void Unregister(PacketHandler.ClientReader reader)
    {
        server.Unregister(reader);
    }

    public override void SendPacket(ISerializable serializable)
    {
        server.SendPacket(serializable);
    }

    public UserList GetUserList() => server.GenerateUserList();

#if UNITY_EDITOR
    private void OnGUI()
    {
        EditorGUIUtility.ScaleAroundPivot(new Vector2(3, 3), Vector2.zero);
        for (int i = server.Clients.Count - 1; i >= 0; i--)
        {
            var client = server.Clients[i];
            GUI.Box(new Rect(0, 300 + (i * 52), 200, 50), client.ID.ToString());
        }
    }
#endif
}


enum ServerStates
{
    Lobby,
    Loading,
    Playing,
    Returning
}
