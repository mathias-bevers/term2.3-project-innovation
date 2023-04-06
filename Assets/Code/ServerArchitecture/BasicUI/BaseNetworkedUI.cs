using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNetworkedUI : NetworkingBehaviour
{
    [SerializeField] CharacterUICanvas canvas;
    
    internal override void Awoken()
    {
        canvas.gameObject.SetActive(false);
    }

    internal void OnUserList(UserList userList)
    {
        canvas.gameObject.SetActive(true);
        canvas.OnUserList(userList);
    }
    
    internal void DeathReceived(DeathEvent e)
    {
        canvas.SetDeathEvent(e);
    }
}
