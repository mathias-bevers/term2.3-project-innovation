using UnityEngine;

public struct UserData 
{
    public int ID;
    public string Name;
    public Vector3 colour;
    public bool ready;

    public UserData(int id) 
    { 
        ID = id; 
        Name = string.Empty; 
        colour = Vector3.one;
        ready = false;
    }

    public UserData(int id, string name)
    {
        ID = id;
        Name = name;
        colour = Vector3.one;
        ready = false;
    }

    public UserData(int id, string name, Vector3 colour)
    {
        ID = id;
        Name = name;
        this.colour = colour;
        ready = false;
    }
}
