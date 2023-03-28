using shared;
using System;
using UnityEngine;

public class DeclareUser : ISerializable
{
    public int ID { get => _ID; }
    public string Name { get => _UserName; }
    public Vector3 Colour { get => _colour; }

    int _ID;
    string _UserName;
    SerializableVector3 _colour;
    
    public DeclareUser() { }

    public DeclareUser(int iD, string userName, Vector3 colour)
    {
        _ID = iD;
        _UserName = userName;
        _colour = colour;
    }

    public override void Deserialize(Packet pPacket)
    {
        _ID = pPacket.ReadInt();
        _UserName = pPacket.ReadString();
        _colour = pPacket.Read<SerializableVector3>();
    }

    public override void Serialize(Packet pPacket)
    {
        pPacket.Write(_ID);
        pPacket.Write(_UserName);
        pPacket.Write(_colour);
    }
}

