using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityInvincibilityAndAttraction : Ability
{

    AudioController controller;

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        player.blockAllDamage = true;
        player.attributes.isPriority = true;
        AudioManager.instance.PlayClipLocalSpace(controller.honeyHulk);
    }

    public override void OnAbilityEnd()
    {
        player.attributes.isPriority = false;
        player.blockAllDamage = false;
        base.OnAbilityEnd();
    }

}
