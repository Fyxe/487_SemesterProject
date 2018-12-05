using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRanged : Weapon
{
    [Header("References")]
    public Bullet bulletPrefab;
    public PooledObject muzzleFlashPrefab;

    [Header("Ranged Weapon Settings")]
    public float projectileDestroyTime = 3f;
    public int amountOfBulletsPerShot;
    public float spread;
    public float knockbackForce;
    public bool isPiercing;
    public bool isExplosive;
    public float explosionRadius;

    [Header("References")]
    public Transform shotPosition;
    public AudioClip shot;
    public ParticleSystem muzzleFlash;


    protected override void Attack(int damageBase, float damageMultipier)
    {
        base.Attack(damageBase, damageMultipier);

        AudioManager.instance.PlayClipLocalSpace(shot);

        //PooledObject spawnedMuzzleFlash = ObjectPoolingManager.instance.CreateObject(muzzleFlashPrefab, null, 0.5f);
        //spawnedMuzzleFlash.transform.position = shotPosition.transform.position;
        //spawnedMuzzleFlash.transform.rotation = shotPosition.transform.rotation;
        for (int i = 0; i < amountOfBulletsPerShot; i++)
        {
            Bullet spawnedBullet = ObjectPoolingManager.instance.CreateObject(bulletPrefab, null, projectileDestroyTime) as Bullet;
            spawnedBullet.damage = (int)((damageBase + damage) * damageMultipier);
            spawnedBullet.layerMask = PlayerManager.instance.layerMaskToShoot;
            //Debug.Log("Weapon damage: " + spawnedBullet.damage);
            spawnedBullet.bounce = controllerCurrent.attachedPlayer.hasBouncingShots;
            spawnedBullet.weaponFiredFrom = this;
            spawnedBullet.transform.position = shotPosition.transform.position;
            spawnedBullet.transform.rotation = shotPosition.transform.rotation;
            Vector3 newDirection = shotPosition.transform.forward;
            Vector3 randomSpread = new Vector3(Random.Range(-spread, spread), 0f);
            newDirection += transform.TransformDirection(randomSpread);            
            spawnedBullet.ShootProjectile(newDirection, speedAttack, 1);
        }

        muzzleFlash.Emit(1);
        controllerCurrent.AddForceToAttachedEntity(-1f * knockbackForce * controllerCurrent.transform.forward, ForceMode.Impulse);

        OnAttack();
    }
}
