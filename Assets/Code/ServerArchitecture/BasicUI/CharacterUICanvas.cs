using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUICanvas : MonoBehaviour
{
    [SerializeField] ColourPannel[] colourPannel;
    public void OnUserList(UserList userList)
    {
        int counter = 0;
        foreach (DeclareUser item in userList.users)
        {
            colourPannel[counter].ID = item.ID;
            colourPannel[counter].SetName(item.Name);
            colourPannel[counter].SetColor(item.Colour);
            counter++;
        }
    }

    public void SetDeathEvent(DeathEvent e)
    {
        foreach(ColourPannel colourPannel in colourPannel)
        {
            if (colourPannel.ID == e.ID)
                colourPannel.SetDeathEvent(e.deathType);
        }
    }


    public void SetBar(BakingPacket packet)
    {
        foreach (BakingPacketData bakingPacket in packet.bakingPackets)
        {
            foreach (ColourPannel colourPannel in colourPannel)
            {
                if (colourPannel.ID != bakingPacket.ID) continue;
                colourPannel.SetBar(bakingPacket.actualAmount, packet.maxBake);
                break;
            }
        }
    }
}
