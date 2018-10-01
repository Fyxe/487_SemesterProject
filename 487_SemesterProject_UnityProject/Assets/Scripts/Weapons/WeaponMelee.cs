using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMelee : Weapon
{
    public Transform hitbox;
    public Transform startRotation;
    public Transform endRotation;
    public Transform rotationAxis;

    protected override void Attack()
    {
        base.Attack();
        StartCoroutine(AttackCoroutine());
    }

    public IEnumerator AttackCoroutine()
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
}
