using shared;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

public class MainServer : MonoBehaviour
{
    IPAddress ipAd = IPAddress.Parse("127.0.0.1");
    TcpListener listener;

    List<TcpClient> clients = new List<TcpClient>();

    public MainServer()
    {
        listener = new TcpListener(ipAd, 25565);
    }

    public void StartServer()
    {
        listener.Start();
    }

    private void Update()
    {
        try
        {
            AcceptNewClients();
            HandleClients();
        }
        catch
        {

        }
    }

    void AcceptNewClients()
    {
        if (!listener.Pending()) return;
        TcpClient curClient = listener.AcceptTcpClient();
        clients.Add(curClient);
    }

    void HandleClients()
    {
        foreach(TcpClient client in clients)
        {
            if (client.Available == 0) continue;
            byte[] gottenBytes = StreamUtil.Read(client.GetStream());
            Packet packet = new Packet(gottenBytes);
            ISerializable current = packet.Read<ISerializable>();

            if (current is Score) Debug.Log("JA!");
        }
    }
}
