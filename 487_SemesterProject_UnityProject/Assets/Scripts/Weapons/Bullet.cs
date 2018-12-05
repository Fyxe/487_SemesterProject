using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    [Header("Settings")]
    public int damage;

    [Header("References")]
    public WeaponRanged weaponFiredFrom;
    public PooledObject onHitParticleSystem;
    PooledObject cachedParticleSystem;
   

    public override void OnHit(Collider hit)
    {
        base.OnHit(hit);
        Damageable damageableHit = hit.gameObject.GetComponentInParent<Damageable>();

        if (onHitParticleSystem != null)
        {
            cachedParticleSystem = ObjectPoolingManager.instance.CreateObject(onHitParticleSystem);
            cachedParticleSystem.transform.position = transform.position;
        }

        if (damageableHit != null)
        {
            bool killed = damageableHit.Hurt(damage);

            if (weaponFiredFrom.isExplosive)
            {
                Collider[] cols = Physics.OverlapSphere(transform.position, weaponFiredFrom.explosionRadius, PlayerManager.instance.layerMaskToShoot);
                //Debug.Log("Explosion hi size: " + cols.Length);
                foreach (Collider col in cols)
                {
                    Damageable damageable = col.GetComponentInParent<Damageable>();
                    if (damageable != null && damageable.team == 0)
                    {
                        //Debug.Log("damageable exists!");
                        bool damageableKilled = damageable.Hurt(damage);

                        if (damageable is AI && weaponFiredFrom.controllerCurrent.attachedPlayer != null)
                        {
                            if (damageableKilled)
                            {
                                weaponFiredFrom.controllerCurrent.attachedPlayer.pointsCurrent += PointsManager.instance.pointsOnEnemyKill;
                                weaponFiredFrom.OnKill(damageable);
                            }
                            else
                            {
                                weaponFiredFrom.controllerCurrent.attachedPlayer.pointsCurrent += PointsManager.instance.pointsOnEnemyHit;
                            }
                            LevelManager.instance.SpawnOnEnemyHit(damageable.transform.position + Vector3.up);
                        }
                    }
                }
            }

            //Debug.Log("hit enemy: "+damageableHit.gameObject.name+" with health: "+damageableHit.hpCurrent);
            if (damageableHit is AI && weaponFiredFrom.controllerCurrent.attachedPlayer != null)
            {
                if (killed)
                {
                    weaponFiredFrom.controllerCurrent.attachedPlayer.pointsCurrent += PointsManager.instance.pointsOnEnemyKill;
                    weaponFiredFrom.OnKill(damageableHit);
                }
                else
                {
                    weaponFiredFrom.controllerCurrent.attachedPlayer.pointsCurrent += PointsManager.instance.pointsOnEnemyHit;
                }
                LevelManager.instance.SpawnOnEnemyHit(damageableHit.transform.position + Vector3.up);
            }            
        }
        else
        {
            //Debug.Log("Hit a not damageable: " + hit.gameObject);
        }
        if (!weaponFiredFrom.isPiercing || damageableHit == null)
        {
            DestroyThisObject();
        }
    }
}
