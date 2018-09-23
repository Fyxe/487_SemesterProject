using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRanged : Weapon
{
    public Projectile projectilePrefab;

    public float projectileSpeed = 1f;
    public float projectileDestroyTime = 3f;
    public LayerMask layerMaskToShoot;

    protected override void Attack()
    {
        base.Attack();
        Projectile spawnedProjectile = ObjectPoolingManager.instance.CreateObject(projectilePrefab, null, projectileDestroyTime) as Projectile;
        spawnedProjectile.transform.position = transform.position;
        spawnedProjectile.ShootProjectile(transform.forward, projectileSpeed, 1);
    }
}
