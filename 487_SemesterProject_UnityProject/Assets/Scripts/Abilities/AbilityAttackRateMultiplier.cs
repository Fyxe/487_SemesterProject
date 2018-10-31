using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAttackRateMultiplier : Ability
{
    [Header("Settings")]
    public float attackRateMultiplier = 2f;

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();

    }

    public override void OnAbilityEnd()
    {

        base.OnAbilityEnd();
    }

}
