using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAttackRateMultiplier : Ability
{
    [Header("Settings")]
    public float attackRateMultiplier = 2f;
    AudioController controller;

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        AudioManager.instance.PlayClipLocalSpace(controller.marmaladeMadness);
    }

    public override void OnAbilityEnd()
    {

        base.OnAbilityEnd();
    }

}
