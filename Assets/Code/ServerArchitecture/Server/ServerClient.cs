using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class ServerClient 
{
    TcpClient _client;
    public TcpClient client { get => _client; }

    int _id;
    public int ID { get => _id; set => _id = value; }

    public int Available => _client.Available;
    public NetworkStream stream => _client.GetStream();

    public int missedHeartbeats = 0;

    public bool markAsDead = false;

    public bool isReady = false;

    public DeclareUser self;

    public ServerClient(TcpClient client, int id)
    {
        _client = client;
        _id = id;
        self = new DeclareUser(id, "Guest" + id.ToString(), ColourType.Blue);
    }

    public static implicit operator TcpClient(ServerClient client) => client.client;
}

