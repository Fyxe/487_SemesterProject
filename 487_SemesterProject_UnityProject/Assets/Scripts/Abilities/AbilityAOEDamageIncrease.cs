﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAOEDamageIncrease : Ability
{

    [Header("Settings")]
    public float newDamageMultiplier = 1.5f;
    public float newDamageMultiplierForOthers = 1.2f;
    public float range = 5f;
    public AudioClip sound;

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        player.damageMultiplier = newDamageMultiplier;
        player.SetOtherPlayersDamageMultiplayer(range,newDamageMultiplierForOthers,durationAttack);
        AudioManager.instance.PlayClipLocalSpace(sound);
    }

    public override void OnAbilityEnd()
    {
        player.damageMultiplier = 1f;
        base.OnAbilityEnd();
    }

}