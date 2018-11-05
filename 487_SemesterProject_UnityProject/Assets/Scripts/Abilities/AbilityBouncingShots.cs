using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBouncingShots : Ability
{

    public AudioClip sound;

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        player.hasBouncingShots = true;
        AudioManager.instance.PlayClipLocalSpace(sound);
    }

    public override void OnAbilityEnd()
    {
        player.hasBouncingShots = false;
        base.OnAbilityEnd();
    }

}
