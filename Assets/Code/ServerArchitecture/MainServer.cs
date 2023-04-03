using shared;
using UnityEngine;
using static PacketHandler;

public class MainServer : IRegistrable
{
    ServerListener server;

    ServerStates serverStates = ServerStates.Lobby;

    public void Awake()
    {
        DontDestroyOnLoad(this);
        server.Declare<RequestNameChange>(new NetworkedCallback(HandleNameChange, TrafficDirection.Received));
        server.Declare<Disconnected>(new NetworkedCallback(HandleDisconnect, TrafficDirection.Both));
        server.Declare<ReadyRequest>(new NetworkedCallback(HandleReady, TrafficDirection.Received));
        server.Declare<AskLoadingEntered>(new NetworkedCallback(HandleLoadingEntered, TrafficDirection.Send));
        StartServer();
    }

    void HandleDisconnect(ServerClient client, ISerializable serializable, TrafficDirection direction)
    {
        server.DisconnectClient(client);
    }

    void HandleClient(ServerClient client, ISerializable serializable, TrafficDirection direction)
    {
        
    }

    void HandleLoadingEntered(ServerClient client, ISerializable serializable, TrafficDirection direction)
    {
        AskLoadingEntered hasLoaded = (AskLoadingEntered)serializable;
    }

    void HandleReady(ServerClient client, ISerializable readyRequest, TrafficDirection direction)
    {
        ReadyRequest ready = (ReadyRequest)readyRequest;
        foreach(ServerClient client2 in server.Clients)
            if(client2.ID == client.ID)
                client2.isReady = ready.Readied;
        server.SendMessages(server.Clients, new ReadyRequest(client.ID, ready.Readied));
    }

    void HandleNameChange(ServerClient client, ISerializable serializable, TrafficDirection direction)
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

    public override ClientReader reader { get => server.reader; set => server.reader = value; }
    public override int ID => server.ID;

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
}


enum ServerStates
{
    Lobby,
    Loading,
    Playing
}
