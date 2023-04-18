using shared;
using UnityEngine;

public class RegisterColour : ISerializable
{
    public int ID { get; set; }
    public ColourType colourType { get; set; }

    public RegisterColour() { }
    public RegisterColour(ColourType type)
    {
        this.colourType = type;
    }
    public RegisterColour(int ID, ColourType colourType)
    {
        this.ID = ID;
        this.colourType = colourType;
    }

    public override void Deserialize(Packet pPacket)
    {
        ID = pPacket.ReadInt();
        colourType = (ColourType)pPacket.ReadInt();
    }

    public override void Serialize(Packet pPacket)
    {
        pPacket.Write(ID);
        pPacket.Write((int)colourType);
    }
}

[System.Serializable]
public enum ColourType
{
    Green,
    Red,
    Blue,
    Yellow
}

[System.Serializable]
public struct ColourMaterial
{
    public ColourType colourType;
    public Material colourMaterial;
}


[System.Serializable]
public struct ColourVector
{
    public ColourType colourType;
    public Color colourMaterial;
}
