using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable {

    public float hpMax;
    public float hpCurrent;

    public bool blockAllDamage;
    public bool blockAllHealing;

    public bool isDead = false;


    public virtual void Hurt(float amount)
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

    public virtual void Heal(float amount)
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
    
    public virtual void Reset()
    {
        HealToMax();
    }
}
