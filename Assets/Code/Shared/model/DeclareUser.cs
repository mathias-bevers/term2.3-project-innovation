using shared;
using System;
using UnityEngine;

public class DeclareUser : ISerializable
{
    public int ID { get => _ID; }
    public string Name { get => _UserName; set => _UserName = value; }
    public ColourType Colour { get => _colour; set => _colour = value; }

    int _ID;
    string _UserName;
    ColourType _colour;
    
    public DeclareUser() { }

    public DeclareUser(int iD, string userName, ColourType colour)
    {
        _ID = iD;
        _UserName = userName;
        _colour = colour;
    }

    public override void Deserialize(Packet pPacket)
    {
        _ID = pPacket.ReadInt();
        _UserName = pPacket.ReadString();
        _colour = pPacket.Read<RegisterColour>().colourType;
    }

    public override void Serialize(Packet pPacket)
    {
        pPacket.Write(_ID);
        pPacket.Write(_UserName);
        pPacket.Write(new RegisterColour(_colour));
    }
}

