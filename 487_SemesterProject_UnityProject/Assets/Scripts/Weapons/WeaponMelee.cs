using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMelee : Weapon
{
    public Transform hitbox;
    public Transform startRotation;
    public Transform endRotation;
    public Transform rotationAxis;
    public MeleeWeaponMode meleeWeaponMode;
    

    protected override void Attack()
    {
        base.Attack();
        switch (meleeWeaponMode)
        {
            case MeleeWeaponMode.swing;
                StartCoroutine(AttackCoroutineSwing());
                break;
            case MeleeWeaponMode.stab:
                StartCoroutine(AttackCoroutineStab());
                break;
            default:
                Debug.LogError("Invalid Melee Weapon Mode");
                break;
        }
    }

    public IEnumerator AttackCoroutineSwing()
    {
        float nextAttack = 0f;
        while (nextAttack < speedAttack)
        {
            nextAttack += Time.deltaTime;
            hitbox.rotation = Quaternion.Lerp(startRotation.rotation, endRotation.rotation, (nextAttack / speedAttack));
            yield return null;
        }
        
        hitbox.position.Set(0, 0, 0);
        hitbox.rotation = startRotation.rotation;
    }

    public IEnumerator AttackCoroutineStab()
    {
        // TODO: do stab
        yield return null;
    }
}
