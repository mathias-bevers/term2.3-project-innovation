using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayHandler : NetworkingBehaviour
{
    [SerializeField] GameplayCharacter character;
    [SerializeField] PointList pointList;
    [SerializeField] Transform arenaMiddle;

    List<GameplayCharacter> spawnedCharacters = new List<GameplayCharacter>();

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
            GameplayCharacter newCharacter = Instantiate(character);
            spawnedCharacters.Add(newCharacter);
            newCharacter.ID = list[i].ID;
            newCharacter.transform.position = pointList[i].position;
            newCharacter.transform.LookAt(arenaMiddle.transform);
        }
    }
}
