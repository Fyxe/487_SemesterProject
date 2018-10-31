using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    [Header("Settings")]
    public int damage;

    [Header("References")]
    public WeaponRanged weaponFiredFrom;
    // CACHE Player

    public override void OnHit(Collider hit)
    {
        base.OnHit(hit);
        Damageable damageableHit = hit.gameObject.GetComponentInParent<Damageable>();
        if (damageableHit != null)
        {
            bool killed = damageableHit.Hurt(damage);            
            if (damageableHit is AI && weaponFiredFrom.controllerCurrent.attachedPlayer != null)
            {
                if (killed)
                {
                    weaponFiredFrom.controllerCurrent.attachedPlayer.pointsCurrent += PointsManager.instance.pointsOnEnemyKill;
                }
                else
                {
                    weaponFiredFrom.controllerCurrent.attachedPlayer.pointsCurrent += PointsManager.instance.pointsOnEnemyHit;
                }
            }
        }
    }
}
