using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    [SerializeField] AudioClipGroup clipGroup;
    [SerializeField] List<EventManager> handlers = new List<EventManager>();

    float timer = 0;
    bool inSpawnEvent = false;

    [Button("Spawn Event")]
    public void SpawnEvent()
    {
        inSpawnEvent = true;
        timer = 0;
        clipGroup?.Play();
    }


    private void Update()
    {
        if (!inSpawnEvent) return;
        timer += Time.deltaTime;
        if(timer > 1)
        {
            timer = 0;
            inSpawnEvent = false;
            handlers[UnityEngine.Random.Range(0, handlers.Count - 1)]?.SpawnEvent();
        }
    }
}
