using shared;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

public class MainServer : MonoBehaviour
{
    [SerializeField] int maxPlayerCount = 4;
    private IPAddress ipAd = IPAddress.Parse("127.0.0.1");
    private ServerListener server;

    private void Awake()
    {
        server.Declare<Score>(HandleScore);
        StartServer();
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

    public void StartServer() => server.Start();
    public void StopServer() => server.Stop();
    public MainServer() => server = new ServerListener(ipAd, 25565, 4);
    private void OnEnable() => server.Register(HandleClient);
    private void OnDisable() => server.Unregister(HandleClient);
    private void Update() => server.Update();
    private void FixedUpdate() => server.FixedUpdate(); 
}
