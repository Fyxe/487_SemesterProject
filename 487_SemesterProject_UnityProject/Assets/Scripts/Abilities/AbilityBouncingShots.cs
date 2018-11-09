using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBouncingShots : Ability
{

    AudioController controller;

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        player.hasBouncingShots = true;
        AudioManager.instance.PlayClipLocalSpace(controller.relishRicochet);
    }

    public override void OnAbilityEnd()
    {
        player.hasBouncingShots = false;
        base.OnAbilityEnd();
    }

}
