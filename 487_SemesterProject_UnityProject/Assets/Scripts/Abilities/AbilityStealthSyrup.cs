using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityStealthSyrup : Ability {

    AudioController controller;

    // Use this for initialization
    public override void OnAbilityStart() {
        AudioManager.instance.PlayClipLocalSpace(controller.stealthSyrup);
        base.OnAbilityStart();
    }

    // Update is called once per frame
    public override void OnAbilityEnd() {
        base.OnAbilityEnd();

    }
}
