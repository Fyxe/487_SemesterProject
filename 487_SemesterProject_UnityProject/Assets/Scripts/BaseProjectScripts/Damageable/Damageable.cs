using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : PooledObject
{

    [Header("Damageable Settings")]
    public bool blockAllDamage;
    public bool blockAllHealing;    
    public virtual int hpMax
    {
        get; set;
    }
    public virtual int hpCurrent
    {
        get; set;
    }
    [Space]
    public bool isDead = false;
    [Space]
    public int team;

    public virtual bool Hurt(int amount)
    {
        if (blockAllDamage)
        {
            return false;
        }

        OnHurt();

        hpCurrent = Mathf.Clamp(hpCurrent - amount,0,hpMax);

        if (hpCurrent == 0)
        {
            isDead = true;
            OnDeath();
            return true;
        }
        return false;
    }

    public virtual void HurtToDeath()
    {
        Hurt(hpCurrent);
    }

    public virtual bool Heal(int amount)
    {
        if (blockAllHealing || hpCurrent == hpMax)
        {
            return false;
        }

        OnHeal();

        hpCurrent = Mathf.Clamp(hpCurrent + amount, 0, hpMax);

        if (hpCurrent == hpMax)
        {            
            OnMaxHeal();
            return true;
        }

        if (hpCurrent > 0)
        {
            isDead = false;
        }
        return false;
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
