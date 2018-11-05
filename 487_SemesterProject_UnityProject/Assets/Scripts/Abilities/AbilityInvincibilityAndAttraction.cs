using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityInvincibilityAndAttraction : Ability
{

    public AudioClip sound;

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        player.blockAllDamage = true;
        player.attributes.isPriority = true;
        AudioManager.instance.PlayClipLocalSpace(sound);
    }

    public override void OnAbilityEnd()
    {
        player.attributes.isPriority = false;
        player.blockAllDamage = false;
        base.OnAbilityEnd();
    }

}
