using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientController : NetworkingBehaviour
{
    int inputCount = 0;
    Vector2 lastInputs;

    float timer = 0;
    readonly float delay = 1.0f / Settings.ticksPerSecond;

    bool dead = false;
    
    public void Update()
    {
        if (dead) return;
        inputCount++;
        lastInputs += new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        timer += Time.deltaTime;
        if(timer >= delay)
        {
            SendPackets();
            timer = 0;
        }
    }

    void SendPackets()
    {
        Vector2 input = lastInputs;
        input.x /= (float)inputCount;
        input.y /= (float)inputCount;
        SerializableVector2 i = new SerializableVector2(input.x, input.y);
        
        SendMessage(new InputPacket(i));
        inputCount = 0;
        lastInputs = Vector2.zero;
    }
}
