using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBouncingShots : Ability
{

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        player.hasBouncingShots = true;
    }

    public override void OnAbilityEnd()
    {
        player.hasBouncingShots = false;
        base.OnAbilityEnd();
    }

}
