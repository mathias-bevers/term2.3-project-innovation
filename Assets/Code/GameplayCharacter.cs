using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameplayCharacter : IDedNetworkingBehaviour
{
    Vector2 lastInput = Vector2.zero;

    [NetworkRegistry(typeof(InputPacket), TrafficDirection.Received)]
    public void Receive(ServerClient client, InputPacket packet, TrafficDirection direction)
    {
        SerializableVector2 input = packet.input ;
        lastInput = new Vector2(input.x, input.y);
        
    }

    private void Update()
    {
        Vector2 input = lastInput * Time.deltaTime * 20;
        transform.position += new Vector3(input.x, 0, input.y);
    }
}
