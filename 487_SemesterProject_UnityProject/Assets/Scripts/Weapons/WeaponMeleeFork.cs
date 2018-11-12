using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMeleeFork : WeaponMelee
{
    [Header("Fork Settings")]
    public int killsToSwitch = 1;
    public int killsCurrent;

    public int swingHitsBeforeTransformation = 3;
    public int swingHitsCurrent;

    public override void OnKill(Damageable damageable)
    {
        base.OnKill(damageable);

        if (meleeWeaponMode == MeleeWeaponMode.stab)
        {
            killsCurrent++;
            if (killsCurrent >= killsToSwitch)
            {
                SwitchMeleeWeaponMode();
                killsCurrent = 0;
            }
        }
    }

    public override void OnAttack()
    {
        base.OnAttack();

        if (meleeWeaponMode == MeleeWeaponMode.swing)
        {
            swingHitsCurrent++;
            if (swingHitsCurrent >= swingHitsBeforeTransformation)
            {
                SwitchMeleeWeaponMode();
                swingHitsCurrent = 0;
            }
        }
    }

    public void SwitchMeleeWeaponMode()
    {
        if (meleeWeaponMode == MeleeWeaponMode.stab)
        {
            meleeWeaponMode = MeleeWeaponMode.swing;
        }
        else if (meleeWeaponMode == MeleeWeaponMode.swing)
        {
            meleeWeaponMode = MeleeWeaponMode.stab;
        }
    }
}
