using UnityEngine;

public struct UserData 
{
    public int ID;
    public string Name;
    public ColourType colour;
    public bool ready;

    public UserData(int id) 
    { 
        ID = id; 
        Name = string.Empty; 
        colour = ColourType.Green;
        ready = false;
    }

    public UserData(int id, string name)
    {
        ID = id;
        Name = name;
        colour = ColourType.Green;
        ready = false;
    }

    public UserData(int id, string name, ColourType colour)
    {
        ID = id;
        Name = name;
        this.colour = colour;
        ready = false;
    }
}
