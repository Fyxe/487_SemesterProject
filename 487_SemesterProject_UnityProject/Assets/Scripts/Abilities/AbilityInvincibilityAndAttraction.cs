using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityInvincibilityAndAttraction : Ability
{
    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        player.blockAllDamage = true;
        player.attributes.isPriority = true;
    }

    public override void OnAbilityEnd()
    {
        player.attributes.isPriority = false;
        player.blockAllDamage = false;
        base.OnAbilityEnd();
    }

}
