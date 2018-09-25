using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : PooledObject
{

    [Header("Damageable Settings")]
    public bool blockAllDamage;
    public bool blockAllHealing;
    [Space]
    public int hpMax;
    public int hpCurrent;
    [Space]
    public bool isDead = false;
    [Space]
    public int team;

    public virtual void Hurt(int amount)
    {
        if (blockAllDamage)
        {
            return;
        }

        OnHurt();

        hpCurrent = Mathf.Clamp(hpCurrent - amount,0,hpMax);

        if (hpCurrent == 0)
        {
            isDead = true;
            OnDeath();
        }
    }

    public virtual void HurtToDeath()
    {
        Hurt(hpCurrent);
    }

    public virtual void Heal(int amount)
    {
        if (blockAllHealing || hpCurrent == hpMax)
        {
            return;
        }

        OnHeal();

        hpCurrent = Mathf.Clamp(hpCurrent + amount, 0, hpMax);

        if (hpCurrent == hpMax)
        {            
            OnMaxHeal();
        }

        if (hpCurrent > 0)
        {
            isDead = false;
        }
    }

    public virtual void HealToMax()
    {
        Heal(hpMax - hpCurrent);
    }

    public virtual void OnHurt()
    {

    }

    public virtual void OnHeal()
    {

    }

    public virtual void OnDeath()
    {

    }

    public virtual void OnMaxHeal()
    {

    }
    
    public float GetHealthPercentage()
    {
        return (float)hpCurrent / (float)hpMax;   
    }

    public virtual void Reset()
    {
        HealToMax();
    }
}
