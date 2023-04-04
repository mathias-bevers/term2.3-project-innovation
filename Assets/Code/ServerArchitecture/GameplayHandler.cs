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
}
