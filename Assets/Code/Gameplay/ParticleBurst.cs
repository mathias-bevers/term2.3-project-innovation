using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleBurst : MonoBehaviour
{
    ParticleSystem system;

    private void Awake()
    {
        system = GetComponent<ParticleSystem>();
    }

    public void OnBurst()
    {
        system.Emit(200);
    }
}
