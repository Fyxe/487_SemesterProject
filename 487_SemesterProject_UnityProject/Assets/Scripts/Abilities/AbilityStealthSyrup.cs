using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityStealthSyrup : Ability {


    // Use this for initialization
    public override void OnAbilityStart() {
        base.OnAbilityStart();
        player.attributes.isInvisible = true;
    }

    // Update is called once per frame
    public override void OnAbilityEnd() {
        base.OnAbilityEnd();
        player.attributes.isInvisible = false;
    }


}
