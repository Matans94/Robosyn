using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] private Transform particles;

    private bool turnOnParticles;
    private void Awake()
    {
        turnOnParticles = false;
        particles.GetComponent<ParticleSystem>().Stop();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (turnOnParticles)
        {
            particles.GetComponent<ParticleSystem>().Emit(1);
            turnOnParticles = false;
        }
        else return;

        if (particles.GetComponent<SphereCollider>().isTrigger)
        {
        }
    }

    private void ParticlesTurnOn()
    {
        turnOnParticles = true;
    }


}
