﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRanged : Weapon
{
    public Bullet bulletPrefab;

    public float projectileDestroyTime = 3f;
    public LayerMask layerMaskToShoot;
    public int amountOfBulletsPerShot;
    public float spread;
    public float knockbackForce;
    public Transform shotPosition;

    protected override void Attack()
    {
        base.Attack();

        for (int i = 0; i < amountOfBulletsPerShot; i++)
        {
            Bullet spawnedBullet = ObjectPoolingManager.instance.CreateObject(bulletPrefab, null, projectileDestroyTime) as Bullet;
            spawnedBullet.weaponFiredFrom = this;
            spawnedBullet.transform.position = shotPosition.transform.position;
            Vector3 newDirection = shotPosition.transform.forward;
            Vector3 randomSpread = new Vector3(Random.Range(-spread, spread), 0f);
            newDirection += transform.TransformDirection(randomSpread);
            spawnedBullet.ShootProjectile(newDirection, speedAttack, 1);
        }
                
        controllerCurrent.AddForceToAttachedEntity(-1f * knockbackForce * controllerCurrent.transform.forward, ForceMode.Impulse);
    }
}
