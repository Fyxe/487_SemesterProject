using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class AIExplode : AI
{
    protected override void Attack()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, attackRadius, PlayerManager.instance.enemyToHitLayerMask);
        List<Damageable> toHurt = new List<Damageable>();
        foreach (var i in cols)
        {
            //Debug.Log((cachedDamageable = i.GetComponentInParent<Damageable>()) != null);
            //Debug.Log(!toHurt.Contains(cachedDamageable));
            //Debug.Log(cachedDamageable.team != team);

            if (!i.isTrigger
                && (cachedDamageable = i.GetComponentInParent<Damageable>()) != null
                && !toHurt.Contains(cachedDamageable)
                && cachedDamageable.team != team)
            {
                toHurt.Add(cachedDamageable);
                cachedDamageable.Hurt(1);
            }
        }
    }
}
