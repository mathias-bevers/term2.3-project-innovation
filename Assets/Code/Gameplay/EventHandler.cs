using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    [SerializeField] float minTime = 10;
    [SerializeField] float maxTime = 30;

    [SerializeField] AudioClipGroup clipGroup;
    [SerializeField] List<EventManager> handlers = new List<EventManager>();

    float timer = 0;
    bool inSpawnEvent = false;

    float eventTimer;

    public void Awake()
    {
        eventTimer = UnityEngine.Random.Range(minTime, maxTime);
    }

    [Button("Spawn Event")]
    public void SpawnEvent()
    {
        inSpawnEvent = true;
        timer = 0;
        clipGroup?.Play();
    }


    private void Update()
    {
        eventTimer -= Time.deltaTime;
        if(eventTimer <= 0)
        {
            eventTimer = UnityEngine.Random.Range(minTime, maxTime);
            SpawnEvent();
        }
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
