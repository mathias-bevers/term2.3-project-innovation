using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayHandler : NetworkingBehaviour
{
    [SerializeField] MarshMallowMovement character;
    [SerializeField] PointList pointList;
    [SerializeField] Transform arenaMiddle;

    List<MarshMallowMovement> spawnedCharacters = new List<MarshMallowMovement>();

    public void Start()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameplayScene"));
        Receive(FindObjectOfType<MainServer>().GetUserList());
    }

    public void Receive(UserList list)
    {
        if (character == null) return;
        for (int i = 0; i < pointList.Count; i++)
        {
            MarshMallowMovement newCharacter = Instantiate(character);
            spawnedCharacters.Add(newCharacter);
            newCharacter.ID = list[i].ID;
            newCharacter.transform.position = pointList[i].position;
            newCharacter.transform.LookAt(arenaMiddle.transform);
        }
    }

#if DEBUG
    int currentSelected = -1;

    public void OnGUI()
    {
        GUIUtility.ScaleAroundPivot(new Vector2(3, 3), Vector2.zero);
        GUI.Box(new Rect(10, 10, 30, 30), currentSelected.ToString());
        for(int i = 0; i < Settings.maxPlayerCount; i++)
        {
            if(GUI.Button(new Rect(10, 42 + (i * 32), 30, 30), new GUIContent(i.ToString(), "Select client number: " + i.ToString())))
            {
                if (i == currentSelected) currentSelected = -1;
                else currentSelected = i;
                FindObjectOfType<FakeClient>()?.SetID(currentSelected);
            }
        }
    }
#endif
}
