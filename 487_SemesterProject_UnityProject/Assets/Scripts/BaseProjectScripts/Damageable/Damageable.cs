using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : PooledObject
{
    [Header("Damageable Settings")]
    public bool blockAllDamage;
    public bool blockAllHealing;
    [SerializeField] int m_hpMax = 0;
    public virtual int hpMax
    {
        get
        {
            return m_hpMax;
        }
        set
        {
            m_hpMax = value;
        }
    }
    [SerializeField] int m_hpCurrent = 0;
    public virtual int hpCurrent
    {
        get
        {
            return m_hpCurrent;
        }
        set
        {
            m_hpCurrent = value;
        }
    }
    [Space]
    public bool isDead = false;
    [Space] // Team 0 is enemy, Team 1 is players
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
