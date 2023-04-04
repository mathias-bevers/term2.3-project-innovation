
using shared;
using UnityEngine;

public class ClientInput : ISerializable
{
    public KeyCode keyCode;
    public bool down;

    public ClientInput() { }
    public ClientInput(KeyCode keyCode, bool down) { this.keyCode = keyCode; this.down = down; }

    public override void Deserialize(Packet pPacket)
    {
        keyCode = (KeyCode)pPacket.ReadInt();
        down = pPacket.ReadBool();
    }

    public override void Serialize(Packet pPacket)
    {
        int val = (int)keyCode;
        pPacket.Write(val);
        pPacket.Write(down);    
    }
}

