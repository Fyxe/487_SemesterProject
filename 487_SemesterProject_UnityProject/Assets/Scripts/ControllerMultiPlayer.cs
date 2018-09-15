using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputController3D))]
public class ControllerMultiPlayer : Damageable
{

    [Header("Settings")]   
    public bool invertAxis1Z = false;
    public float reviveRadius = 2f;

    [Header("Delays")]
    public float delaySwapWeapon = 0.1f;
    float nextSwapWeapon = 0f;
    public float delayUseAbility = 0.1f;
    float nextUseAbility = 0f;
    public float delayRevive = 0f;
    float nextRevive = 0f;
    public float delayThrowWeapon = 0.1f;
    float nextThrowWeapon = 0f;
    public float delayInteract = 0.1f;
    float nextInteract = 0f;
    public float delayAttack = 0f;
    float nextAttack = 0f;
    public float delayDisplayInformation = 0f;
    float nextDisplayInformation = 0f;
    public float delayThrowPoints = 0.1f;
    float nextThrowPoints = 0f;

    [Header("References")]
    PlayerState m_state = PlayerState.disconnected;
    public PlayerState state
    {
        get
        {
            return m_state;
        }
        set
        {
            m_state = value;
            if (state == PlayerState.dead)
            {
                isDead = true;
            }
            else if (state == PlayerState.alive)
            {
                isDead = false;
            }

            if (ui != null)
            {
                switch (state)
                {
                    case PlayerState.alive:
                        ui.Set(PlayerUIBox.BoxSetting.alive);
                        break;
                    case PlayerState.incapacitated:
                        ui.Set(PlayerUIBox.BoxSetting.incapacitated);
                        break;
                    case PlayerState.dead:
                        ui.Set(PlayerUIBox.BoxSetting.dead);
                        break;
                    case PlayerState.disconnected:
                        ui.Set(PlayerUIBox.BoxSetting.empty);
                        break;
                    default:
                        break;
                }
            }
        }
    }
    public PlayerUIBox ui;
    Color m_colorPlayer;
    public Color colorPlayer
    {
        get
        {
            return m_colorPlayer;
        }
        set
        {
            m_colorPlayer = value;
            ui.imageHealthBar.color = colorPlayer;
        }
    }
    public int indexJoystick = 0;
    public int indexPlayer = 0;
    int m_pointsCurrent = 0;
    public int pointsCurrent
    {
        get
        {
            return m_pointsCurrent;
        }
        set
        {
            m_pointsCurrent = value;
            ui.textPoints.text = pointsCurrent.ToString();
        }
    }  
    float m_speedMoveCurrent = 3f;
    public float speedMoveCurrent
    {
        get
        {
            return m_speedMoveCurrent;
        }
        set
        {
            m_speedMoveCurrent = value;
            controller.speedMove = speedMoveCurrent;
            ui.textSpeedMove.text = speedMoveCurrent.ToString();
        }
    }
    int m_damageBaseCurrent = 0;
    public int damageBaseCurrent
    {
        get
        {
            return m_damageBaseCurrent;
        }
        set
        {
            m_damageBaseCurrent = value;
            ui.textDamageBase.text = damageBaseCurrent.ToString();
        }
    }

    int m_countReviveCurrent = 1;
    public int countReviveCurrent
    {
        get
        {
            return m_countReviveCurrent;
        }
        set
        {
            m_countReviveCurrent = value;
            ui.textReviveCount.text = countReviveCurrent.ToString();
        }
    }

    int revivesRemaining = 0;

    InputController3D controller;
    Animator anim;
    Coroutine coroutineInvulnerable;
    Coroutine coroutineIncapacitate;

    void Awake()
    {
        controller = GetComponent<InputController3D>();      
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (state != PlayerState.alive)
        {            
            ui.imageReviveCount.fillAmount = Mathf.Lerp(ui.imageReviveCount.fillAmount,(float)revivesRemaining / (float)countReviveCurrent,0.2f);
            controller.SetAxis(0f, 0f, 0f, 0f);
            return;
        }

        if (ui != null)
        {
            ui.imageHealthBar.fillAmount = Mathf.Lerp(ui.imageHealthBar.fillAmount,GetHealthPercentage(),0.2f);
            if (ui.imageHealthBar.color.a == 0)
            {
                Debug.LogWarning("Healthbar color is transparent.");
            }
        }

        if (invertAxis1Z)
        {
            controller.SetAxis(
                Input.GetAxis("P" + indexJoystick.ToString() + "_Axis0Horizontal"),
                Input.GetAxis("P" + indexJoystick.ToString() + "_Axis0Vertical"),
                Input.GetAxis("P" + indexJoystick.ToString() + "_Axis1Horizontal"),
                Input.GetAxis("P" + indexJoystick.ToString() + "_Axis1Vertical") * (-1f)
                );
        }
        else
        {        
            controller.SetAxis(
                Input.GetAxis("P" + indexJoystick.ToString() + "_Axis0Horizontal"),
                Input.GetAxis("P" + indexJoystick.ToString() + "_Axis0Vertical"),
                Input.GetAxis("P" + indexJoystick.ToString() + "_Axis1Horizontal"),
                Input.GetAxis("P" + indexJoystick.ToString() + "_Axis1Vertical")
                );
        }

        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 0"))
        {
            AttemptSwapWeapons();
        }
        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 1"))
        {
            AttemptUseAbility();
        }
        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 2"))
        {
            AttemptRevive();   
        }
        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 3"))
        {
            AttemptThrowWeapon();
        }
        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 4"))
        {
            AttemptInteract();
        }
        if (Input.GetKey("joystick " + indexJoystick.ToString() + " button 5"))
        {
            AttemptAttack();
        }
        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 6"))
        {
            AttemptDisplayMoreInformation();
        }        
        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 7"))
        {
            // pause menu?
            HurtToDeath();
        }
        if (Input.GetKey("joystick " + indexJoystick.ToString() + " button 8") && Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 9"))
        {
            AttemptThrowPoints();
        }
    }

    public void Setup(PlayerAttributes newAttribute, PlayerUIBox newUI)
    {        
        ui = newUI;

        state = PlayerState.alive;

        colorPlayer = newAttribute.colorPlayer;
        indexJoystick = newAttribute.indexJoystick;
        indexPlayer = newAttribute.indexPlayer;
        pointsCurrent = newAttribute.pointsCurrent;
        hpCurrent = newAttribute.hpCurrent;
        hpMax = newAttribute.hpMax;
        speedMoveCurrent = newAttribute.speedMoveCurrent;
        damageBaseCurrent = newAttribute.damageBaseCurrent;
        countReviveCurrent = newAttribute.countReviveCurrent;

        ui.Set(PlayerUIBox.BoxSetting.alive);

        foreach (var i in GetComponentsInChildren<Renderer>())
        {
            i.material.color = colorPlayer;
        }        
    }

    public void AttemptInteract()
    {
        if (Time.time > nextInteract)
        {
            nextInteract = Time.time + delayInteract;
            Debug.Log("P" + indexJoystick.ToString() + " interacted.");
            if (anim != null)
            {
                anim.SetTrigger("interact");
            }
        }        
    }

    public void AttemptRevive()
    {
        if (Time.time > nextRevive)
        {
            nextRevive = Time.time + delayRevive;
            Debug.Log("P" + indexJoystick.ToString() + " hit a revive.");   
            if (anim != null)
            {
                anim.SetTrigger("revive");
            }

            int mask = 1 << LayerMask.NameToLayer("Player");
            Collider[] cols = Physics.OverlapSphere(transform.position, reviveRadius, mask);            

            ControllerMultiPlayer cachedController = null;
            ControllerMultiPlayer closest = null;
            float dist = float.MaxValue;
            foreach (var i in cols)
            {
                if (!i.isTrigger && (cachedController = i.GetComponentInParent<ControllerMultiPlayer>()) != null && cachedController != this)
                {
                    float newDist = Vector3.Distance(transform.position, i.transform.position);
                    if (newDist < dist)
                    {
                        closest = cachedController;
                        dist = newDist;
                    }
                }
            }

            if (closest != null)
            {
                Debug.Log(closest.name);
                closest.AttemptReviveThisPlayer();
            }
        }        
    }

    public void AttemptAttack()
    {
        if (Time.time > nextAttack)
        {
            nextAttack = Time.time + delayAttack;
            Debug.Log("P" + indexJoystick.ToString() + " Attacked.");   
            if (anim != null)
            {
                anim.SetTrigger("attack");
            }
        }        
    }

    public void AttemptSwapWeapons()
    {
        if (Time.time > nextSwapWeapon)
        {
            nextSwapWeapon = Time.time + delaySwapWeapon;
            Debug.Log("P" + indexJoystick.ToString() + " swapped weapons.");   
            if (anim != null)
            {
                anim.SetTrigger("swapWeapons");
            }
        }        
    }

    public void AttemptThrowWeapon()
    {
        if (Time.time > nextThrowWeapon)
        {
            nextThrowWeapon = Time.time + delayThrowWeapon;
            Debug.Log("P" + indexJoystick.ToString() + " threw their weapon.");
            if (anim != null)
            {
                anim.SetTrigger("throwWeapon");
            }
        }        
    }

    public void AttemptUseAbility()
    {
        if (Time.time > nextUseAbility)
        {
            nextUseAbility = Time.time + delayUseAbility;
            Debug.Log("P" + indexJoystick.ToString() + " used an ability.");   
            if (anim != null)
            {
                anim.SetTrigger("useAbility");
            }
        }        
    }

    public void AttemptThrowPoints()
    {
        if (Time.time > nextThrowPoints)
        {
            nextThrowPoints = Time.time + delayThrowPoints;
            Debug.Log("P" + indexJoystick.ToString() + " threw points.");    
            if (anim != null)
            {
                anim.SetTrigger("throwPoints");
            }
        }        
    }

    public void AttemptDisplayMoreInformation()
    {
        if (Time.time > nextDisplayInformation)
        {
            nextDisplayInformation = Time.time + delayDisplayInformation;
            Debug.Log("P" + indexJoystick.ToString() + " displayed more HUD information.");

            if (ui != null)
            {
                ui.ToggleSize();
            }
        }    
    }

    public void SetInvulnerable(float timeInvulnerable)
    {
        if (coroutineInvulnerable != null)
        {            
            StopCoroutine(coroutineInvulnerable);
        }        
        coroutineInvulnerable = StartCoroutine(Invulerability(timeInvulnerable));
    }

    IEnumerator Invulerability(float timeInvulnerable)
    {
        blockAllDamage = true;
        foreach (var i in GetComponentsInChildren<Renderer>())
        {
            i.material.color = Color.white;
        } 
        yield return new WaitForSeconds(timeInvulnerable);
        blockAllDamage = false;
        foreach (var i in GetComponentsInChildren<Renderer>())
        {
            i.material.color = colorPlayer;
        } 
    }

    public void AttemptReviveThisPlayer()
    {
        if (state != PlayerState.incapacitated)
        {
            return;
        }
        revivesRemaining++;
        if (revivesRemaining >= countReviveCurrent)
        {
            Revive();
        }
    }

    public void Revive()
    {
        countReviveCurrent *= 2;
        revivesRemaining = 0;
        StopCoroutine(coroutineIncapacitate);
        state = PlayerState.alive;
        hpCurrent = 1;
        SetInvulnerable(PlayerManager.instance.timeInvulnerable);
    }

    public override void OnDeath()
    {
        countReviveCurrent *= 2;
        ui.Set(PlayerUIBox.BoxSetting.dead);
        LevelManager.instance.CheckIfAllPlayersAreDead();
    }

    void OnIncapacitate()        
    {
        ui.imageReviveCount.fillAmount = 0;
        ui.imageReviveTimer.fillAmount = 1;
        revivesRemaining = 0;        
    }

    public override void Hurt(int amount)
    {
        if (blockAllDamage || state != PlayerState.alive)
        {
            return;
        }

        OnHurt();

        hpCurrent = Mathf.Clamp(hpCurrent - amount, 0, hpMax);

        if (hpCurrent == 0)
        {
            if (coroutineIncapacitate != null)
            {
                StopCoroutine(coroutineIncapacitate);
            }
            coroutineIncapacitate = StartCoroutine(Incapacitate());            
        }
    }

    IEnumerator Incapacitate()
    {
        state = PlayerState.incapacitated;
        OnIncapacitate();        
        float currentTime = 0f;
        float waitTime = PlayerManager.instance.timeIncapacitated;
        while (currentTime < waitTime)
        {
            currentTime += Time.deltaTime;
            if (ui != null)
            {
                ui.imageReviveTimer.fillAmount =  Mathf.Abs(1f - (currentTime / waitTime));
            }
            yield return null;
        }        
        state = PlayerState.dead;
        isDead = true;
        OnDeath();
    }
}
