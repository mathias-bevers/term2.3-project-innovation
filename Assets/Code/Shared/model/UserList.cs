﻿using shared;

public class UserList : ISerializable
{
    public DeclareUser[] users;

    public override void Deserialize(Packet pPacket)
    {
        users = new DeclareUser[pPacket.ReadInt()];
        for(int i = 0; i < users.Length; i++)
        {
            users[i] = pPacket.Read<DeclareUser>();
        }
    }

    public override void Serialize(Packet pPacket)
    {
        pPacket.Write(users.Length);
        for(int i = 0; i < users.Length; i++)
        {
            pPacket.Write(users[i]);
        }
    }
}

