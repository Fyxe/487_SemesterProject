using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRanged : Weapon
{
    public Projectile projectilePrefab;

    public float projectileDestroyTime = 3f;
    public LayerMask layerMaskToShoot;
    public int amountOfBulletsPerShot;
    public float spread;
    public float knockbackForce;

    protected override void Attack()
    {
        base.Attack();

        for (int i = 0; i < amountOfBulletsPerShot; i++)
        {
            Projectile spawnedProjectile = ObjectPoolingManager.instance.CreateObject(projectilePrefab, null, projectileDestroyTime) as Projectile;
            spawnedProjectile.transform.position = transform.position;
            Vector3 newDirection = transform.forward;
            newDirection += new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), newDirection.z);
            spawnedProjectile.ShootProjectile(newDirection, speedAttack, 1);
        }
                
        currentPlayer.rb.AddForce(-1f * knockbackForce * currentPlayer.transform.forward, ForceMode.Impulse);
    }
}
