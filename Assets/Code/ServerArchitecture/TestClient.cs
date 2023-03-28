using shared;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class TestClient : MonoBehaviour
{
    private IPAddress ipAd = IPAddress.Parse("127.0.0.1");
    TcpClient client = new TcpClient();

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C)) client.Connect(ipAd, 25565);

        if (Input.GetKeyDown(KeyCode.Space))
            SendPacket(new shared.Score("testScore", 99));

        if (Input.GetKeyDown(KeyCode.G))
            SendPacket(new shared.AddRequest());

        if (Input.GetKeyDown(KeyCode.B))
            SendPacket(new shared.GetRequest());
    }

    void SendPacket(ISerializable serializable)
    {
        Packet packet = new Packet();
        packet.Write(serializable);
        StreamUtil.Write(client.GetStream(), packet.GetBytes());
    }
}
