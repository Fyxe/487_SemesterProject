using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class IndicatorParticleSystem : Indicator
{
    new ParticleSystem particleSystem;

    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    public override void Indicate()
    {
        particleSystem.Play();
    }
}
