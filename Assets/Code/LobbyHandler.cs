using shared;
using UnityEngine;

public class LobbyHandler : NetworkingBehaviour
{
    [Space(10)]
    [SerializeField] LobbyCharacter lobbyCharacterPrefab;

    [SerializeField]
    Transform[] spawnPoints;



    [NetworkRegistry(typeof(DeclareUser))]
    public void Receive(ServerClient client, DeclareUser user)
    {
        Debug.Log(user.Name);
    }


    [NetworkRegistry(typeof(UserList))]
    public void Receive(ServerClient client, UserList list)
    {
        foreach(DeclareUser item in list.users)
        {
            Debug.Log(item.Name);
        }
    }
}
