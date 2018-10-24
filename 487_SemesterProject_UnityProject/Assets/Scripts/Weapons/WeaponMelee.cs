using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMelee : Weapon
{
    public Transform hitbox;
    public Transform startRotation;
    Quaternion rotationToResetTo;
    public Transform endRotation;
    public MeleeWeaponMode meleeWeaponMode;
    
    protected override void Attack()
    {
        base.Attack();
        switch (meleeWeaponMode)
        {
            case MeleeWeaponMode.swing:
                StartCoroutine(AttackCoroutineSwing());
                break;
            case MeleeWeaponMode.stab:
                StartCoroutine(AttackCoroutineStab());
                break;
            default:
                Debug.LogError("Invalid Melee Weapon Mode");
                break;
        }
    }

    public IEnumerator AttackCoroutineSwing()
    {
        rotationToResetTo = transform.rotation;
        float currentTime = 0f;
        List<Collider> enemiesHit = new List<Collider>();

        while (currentTime < speedAttack)
        {
            currentTime += Time.deltaTime;
            
            RaycastHit m_Hit;

            hitbox.rotation = Quaternion.Slerp(startRotation.rotation, endRotation.rotation, (currentTime / speedAttack));
            if (Physics.BoxCast(GetComponentInChildren<Collider>().bounds.center, transform.localScale, transform.forward, out m_Hit, transform.rotation, rangeAttack))
            {
                //Output the name of the Collider your Box hit
                Debug.Log("Hit : " + m_Hit.collider.name + " is damageable?: " + (m_Hit.collider.gameObject.GetComponentInParent<Damageable>() != null) + " has enemy tag?: " + (m_Hit.collider.gameObject.tag == "Enemy") + " has been hit before?: " + enemiesHit.Contains(m_Hit.collider));
                if (m_Hit.collider.gameObject.GetComponentInParent<Damageable>() != null && m_Hit.collider.gameObject.tag == "Enemy" && !enemiesHit.Contains(m_Hit.collider))
                {
                    m_Hit.collider.gameObject.GetComponentInParent<Damageable>().HurtToDeath();
                    enemiesHit.Add(m_Hit.collider);
                }
            }


            yield return null;
        }
        
        hitbox.position.Set(0, 0, 0);
        hitbox.rotation = rotationToResetTo;
        enemiesHit.Clear();
    }

    public IEnumerator AttackCoroutineStab()
    {
        // TODO: do stab
        yield return null;
    }

    void OnDrawGizmos()
    {
        
    }
}
