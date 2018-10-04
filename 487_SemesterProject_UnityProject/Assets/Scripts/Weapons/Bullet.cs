﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    public WeaponRanged weaponFiredFrom;

    public override void OnHit(Collider hit)
    {
        base.OnHit(hit);
        Damageable damageableHit = hit.gameObject.GetComponentInParent<Damageable>();
        if (damageableHit != null)
        {
            damageableHit.Hurt(weaponFiredFrom.damage);
            DestroyThisObject();
        }
    }
}
