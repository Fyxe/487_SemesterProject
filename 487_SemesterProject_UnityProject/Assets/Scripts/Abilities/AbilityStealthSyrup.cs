using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityStealthSyrup : Ability {

    public AudioClip sound;

    // Use this for initialization
    public override void OnAbilityStart() {
        AudioManager.instance.PlayClipLocalSpace(sound);
        base.OnAbilityStart();
    }

    // Update is called once per frame
    public override void OnAbilityEnd() {
        base.OnAbilityEnd();

    }
}
