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
    
    protected override void Attack(int damageBase, float damageMultipier)
    {
        base.Attack(damageBase, damageMultipier);
        switch (meleeWeaponMode)
        {
            case MeleeWeaponMode.swing:
                StartCoroutine(AttackCoroutineSwing(damageBase, damageMultipier));
                break;
            case MeleeWeaponMode.stab:
                StartCoroutine(AttackCoroutineStab(damageBase, damageMultipier));
                break;
            default:
                Debug.LogError("Invalid Melee Weapon Mode");
                break;
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
            if (true)//Physics.OverlapBox())//(GetComponentInChildren<Collider>().bounds.center, transform.localScale, transform.forward, out cachedHit, transform.rotation, rangeAttack, layerMaskToHit.value))
            {
                //cachedHit.collider.gameObject.GetComponent<Renderer>().material.color = Color.red;
                if (((cachedDamageable = cachedHit.collider.gameObject.GetComponentInParent<Damageable>()) != null) && !enemiesHit.Contains(cachedDamageable))
                {
                    cachedDamageable.Hurt((int)((damageBase + damage) * damageMultipier));
                    enemiesHit.Add(cachedDamageable);
                }
            }


            yield return null;
        }
        
        modelRotationParent.position.Set(0, 0, 0);
        modelRotationParent.localRotation = Quaternion.identity;
        enemiesHit.Clear();
    }

    public IEnumerator AttackCoroutineStab(int damageBase, float damageMultipier)
    {
        // TODO: do stab
        yield return null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        if (Physics.BoxCast(GetComponentInChildren<Collider>().bounds.center, transform.localScale, transform.forward, transform.rotation, rangeAttack, layerMaskToHit.value))
        {
            Gizmos.DrawRay(transform.position, transform.forward * cachedHit.distance);
            Gizmos.DrawWireCube(transform.position + transform.forward * cachedHit.distance, transform.localScale);
        }
        else
        {
            Gizmos.DrawRay(transform.position, transform.forward * rangeAttack);
            Gizmos.DrawWireCube(transform.position + transform.forward * rangeAttack, transform.localScale);
        }
    }
}
