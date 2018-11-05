using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAttackRateMultiplier : Ability
{
    [Header("Settings")]
    public float attackRateMultiplier = 2f;
    public AudioClip sound;

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        AudioManager.instance.PlayClipLocalSpace(sound);
    }

    public override void OnAbilityEnd()
    {

        base.OnAbilityEnd();
    }

}
