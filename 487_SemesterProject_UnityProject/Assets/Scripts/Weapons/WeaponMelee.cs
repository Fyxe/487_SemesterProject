using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMelee : Weapon
{
    [Header("Melee Weapon Settings")]
    public MeleeWeaponMode meleeWeaponMode;
    public LayerMask layerMaskToHit;

    [Header("References")]
    public Transform modelRotationParent;
    public Transform startRotation;
    public Transform endRotation;

    Damageable cachedDamageable;
    RaycastHit cachedHit;
    Coroutine AttackingCoroutine;
    public AudioClip soundOne;
    public AudioClip soundTwo;

    protected override void Attack(int damageBase, float damageMultipier)
    {
        if(Random.value < 0.5) {
            AudioManager.instance.PlayClipLocalSpace(soundOne);
        } else {
            AudioManager.instance.PlayClipLocalSpace(soundTwo);
        }
        base.Attack(damageBase, damageMultipier);
        if (AttackingCoroutine == null)
        {
            switch (meleeWeaponMode)
            {
                case MeleeWeaponMode.swing:
                    AttackingCoroutine = StartCoroutine(AttackCoroutineSwing(damageBase, damageMultipier));
                    break;
                case MeleeWeaponMode.stab:
                    AttackingCoroutine = StartCoroutine(AttackCoroutineStab(damageBase, damageMultipier));
                    break;
                default:
                    Debug.LogError("Invalid Melee Weapon Mode");
                    break;
            }
        }
    }

    public IEnumerator AttackCoroutineSwing(int damageBase, float damageMultipier)
    {
        float currentTime = 0f;
        List<Damageable> enemiesHit = new List<Damageable>();

        while (currentTime < speedAttack)
        {
            currentTime += Time.deltaTime;

            modelRotationParent.rotation = Quaternion.Lerp(startRotation.rotation, endRotation.rotation, (currentTime / speedAttack));
            Bounds weaponBounds = GetComponentInChildren<Collider>().bounds;
            Collider[] hitColliders = Physics.OverlapBox(new Vector3(weaponBounds.center.x, weaponBounds.center.y, weaponBounds.center.z), new Vector3(transform.localScale.x / 2, transform.localScale.y / 2, (transform.localScale.z / 2)), modelRotationParent.rotation, layerMaskToHit.value);
            foreach (Collider i in hitColliders)
            {
                //i.gameObject.GetComponent<Renderer>().material.color = Color.red;
                cachedDamageable = i.gameObject.GetComponentInParent<Damageable>();
                if (cachedDamageable != null)//(GetComponentInChildren<Collider>().bounds.center, transform.localScale, transform.forward, out cachedHit, transform.rotation, rangeAttack, layerMaskToHit.value))
                {
                    if (!enemiesHit.Contains(cachedDamageable))
                    {
                        cachedDamageable.Hurt((int)((damageBase + damage) * damageMultipier));
                        enemiesHit.Add(cachedDamageable);
                    }
                }
            }

            yield return null;
        }

        modelRotationParent.position.Set(0, 0, 0);
        modelRotationParent.localRotation = Quaternion.identity;
        enemiesHit.Clear();
        AttackingCoroutine = null;
    }

    public IEnumerator AttackCoroutineStab(int damageBase, float damageMultipier)
    {
        float currentTime = 0f;
        List<Damageable> enemiesHit = new List<Damageable>();

        while (currentTime < speedAttack)
        {
            currentTime += Time.deltaTime;

            modelRotationParent.transform.position += transform.forward * rangeAttack * Time.deltaTime;
            Bounds weaponBounds = GetComponentInChildren<Collider>().bounds;
            Collider[] hitColliders = Physics.OverlapBox(new Vector3(weaponBounds.center.x, weaponBounds.center.y, weaponBounds.center.z), new Vector3(transform.localScale.x / 2, transform.localScale.y / 2, (transform.localScale.z / 2)), modelRotationParent.rotation, layerMaskToHit.value);
            foreach (Collider i in hitColliders)
            {
                //i.gameObject.GetComponent<Renderer>().material.color = Color.red;
                cachedDamageable = i.gameObject.GetComponentInParent<Damageable>();
                if (cachedDamageable != null)//(GetComponentInChildren<Collider>().bounds.center, transform.localScale, transform.forward, out cachedHit, transform.rotation, rangeAttack, layerMaskToHit.value))
                {
                    if (!enemiesHit.Contains(cachedDamageable))
                    {
                        cachedDamageable.Hurt((int)((damageBase + damage) * damageMultipier));
                        enemiesHit.Add(cachedDamageable);
                    }
                }
            }

            yield return null;
        }


        currentTime = 0f;

        while (currentTime < speedAttack)
        {
            currentTime += Time.deltaTime;
            modelRotationParent.transform.position -= transform.forward * rangeAttack * Time.deltaTime;

            yield return null;
        }


        //modelRotationParent.transform.position = new Vector3(0,0,0);
        modelRotationParent.transform.position.Set(0, 0, 0);
        //modelRotationParent.localRotation = Quaternion.identity;
        enemiesHit.Clear();
        AttackingCoroutine = null;

    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;

        //if (Physics.BoxCast(GetComponentInChildren<Collider>().bounds.center, transform.localScale, transform.forward, transform.rotation, rangeAttack, layerMaskToHit.value))
        //{
        //    Gizmos.DrawRay(transform.position, transform.forward * cachedHit.distance);
        //    Gizmos.DrawWireCube(transform.position + transform.forward * cachedHit.distance, transform.localScale);
        //}
        //else
        //{
        //    Gizmos.DrawRay(transform.position, transform.forward * rangeAttack);
        //    Gizmos.DrawWireCube(transform.position + transform.forward * rangeAttack, transform.localScale);
        //}
    }
}
