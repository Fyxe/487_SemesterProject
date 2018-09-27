using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputController3D))]
public class ControllerMultiPlayer : Damageable
{

    public override int hpCurrent
    {
        get
        {
            return attributes.hpCurrent;
        }
        set
        {
            attributes.hpCurrent = value;
            ui.SetHealth(hpCurrent, hpMax, attributes.colorPlayer);
        }
    }

    public override int hpMax
    {
        get
        {
            return attributes.hpMax;
        }
        set
        {
            attributes.hpMax = value;
            ui.SetHealth(hpCurrent, hpMax, attributes.colorPlayer);
        }
    }

    [Header("Settings")]   
    public bool invertAxis1Z = false;
    public float reviveRadius = 2f;
    public float interactRadius = 1f;
    public float forceThrow = 50f;

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
    public float delayAttackAlternate = 0f;
    float nextAttackAlternate = 0f;
    public float delayDisplayInformation = 0f;
    float nextDisplayInformation = 0f;
    public float delayThrowPoints = 0.1f;
    float nextThrowPoints = 0f;

    [Header("References")]
    PlayerState m_state = PlayerState.disconnected;
    public Transform positionThrow;
    public GameObject incapacitatedObject;
    public Weapon weaponCurrent;
    public Transform positionWeaponCurrent;
    public Queue<Weapon> weaponsUnequipped = new Queue<Weapon> ();
    public List<Transform> positionsWeaponsUnequipped = new List<Transform>();
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
                        incapacitatedObject.SetActive(false);
                        break;
                    case PlayerState.incapacitated:
                        ui.Set(PlayerUIBox.BoxSetting.incapacitated);
                        incapacitatedObject.SetActive(true);
                        break;
                    case PlayerState.dead:
                        ui.Set(PlayerUIBox.BoxSetting.dead);
                        incapacitatedObject.SetActive(false);
                        break;
                    case PlayerState.disconnected:
                        ui.Set(PlayerUIBox.BoxSetting.empty);
                        incapacitatedObject.SetActive(false);
                        break;
                    default:
                        break;
                }
            }
        }
    }
    public PlayerUIBox ui;
    public Color colorPlayer
    {
        get
        {
            return attributes.colorPlayer;
        }
        set
        {
            attributes.colorPlayer = value;
            ui.SetHealth(hpCurrent, hpMax, attributes.colorPlayer);
        }
    }
    public int pointsCurrent
    {
        get
        {
            return attributes.pointsCurrent;
        }
        set
        {
            attributes.pointsCurrent = value;
            ui.textPoints.text = pointsCurrent.ToString();
        }
    }  
    public float speedMoveCurrent
    {
        get
        {
            return attributes.speedMoveCurrent;
        }
        set
        {
            attributes.speedMoveCurrent = value;
            controller.speedMove = speedMoveCurrent;
            ui.textSpeedMove.text = speedMoveCurrent.ToString();
        }
    }    
    public int damageBaseCurrent
    {
        get
        {
            return attributes.damageBaseCurrent;
        }
        set
        {
            attributes.damageBaseCurrent = value;
            ui.textDamageBase.text = damageBaseCurrent.ToString();
        }
    }    
    public int countReviveCurrent
    {
        get
        {
            return attributes.countReviveCurrent;
        }
        set
        {
            attributes.countReviveCurrent = value;
            ui.textReviveCount.text = countReviveCurrent.ToString();
        }
    }

    int revivesRemaining = 0;

    int indexJoystick
    {
        get
        {
            return attributes.indexJoystick;
        }
    }
    int indexPlayer
    {
        get
        {
            return attributes.indexPlayer;
        }
    }

    PlayerAttributes attributes;
    InputController3D controller;
    Animator anim;
    Coroutine coroutineInvulnerable;
    Coroutine coroutineIncapacitate;
    public List<Projector> projectors = new List<Projector>();
    bool triggerInUseRight = false;
    bool triggerInUseLeft = false;

    void Awake()
    {        
        controller = GetComponent<InputController3D>();      
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Hurt(1);
        }

        if (state != PlayerState.alive)
        {            
            ui.imageReviveCount.fillAmount = Mathf.Lerp(ui.imageReviveCount.fillAmount,(float)revivesRemaining / (float)countReviveCurrent,0.2f);
            controller.SetAxis(0f, 0f, 0f, 0f);
            return;
        }

        

        if (invertAxis1Z)
        {
            controller.SetAxis(
                Input.GetAxis("J" + indexJoystick.ToString() + "_Axis0Horizontal"),
                Input.GetAxis("J" + indexJoystick.ToString() + "_Axis0Vertical"),
                Input.GetAxis("J" + indexJoystick.ToString() + "_Axis1Horizontal"),
                Input.GetAxis("J" + indexJoystick.ToString() + "_Axis1Vertical") * (-1f)
                );
        }
        else
        {        
            controller.SetAxis(
                Input.GetAxis("J" + indexJoystick.ToString() + "_Axis0Horizontal"),
                Input.GetAxis("J" + indexJoystick.ToString() + "_Axis0Vertical"),
                Input.GetAxis("J" + indexJoystick.ToString() + "_Axis1Horizontal"),
                Input.GetAxis("J" + indexJoystick.ToString() + "_Axis1Vertical")
                );
        }

        if (Mathf.Abs(Input.GetAxis("J" + indexJoystick.ToString() + "_Axis1Horizontal")) + 
            Mathf.Abs(Input.GetAxis("J" + indexJoystick.ToString() + "_Axis1Vertical")) > 0)
        {
            AttemptAttack();
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

        if (Input.GetKey("joystick " + indexJoystick.ToString() + " button 4"))
        {            
            controller.lookBehind = true;
            AttemptAttack();
        }
        else
        {
            controller.lookBehind = false;
        }

        if (Input.GetAxis("J" + indexJoystick.ToString() + "_ButtonTrigger") > 0)
        {            
            AttemptAttackAlternate();
        }


        
        if (Input.GetAxis("J" + indexJoystick.ToString() + "_ButtonTrigger") < 0)
        {            
            if (!triggerInUseLeft)
            {
                triggerInUseLeft = true;
                AttemptInteract();
            }            
        }
        else
        {
            triggerInUseLeft = false;
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

    public void Setup(PlayerAttributes newAttributes, PlayerUIBox newUI)
    {        
        if (newAttributes.indexJoystick == 0)
        {
            Debug.LogError("Cannot have a joystick zero.");
            return;
        }
        this.name = "[J" + newAttributes.indexJoystick.ToString() + ":P" + newAttributes.indexPlayer.ToString() + "]Controller";

        attributes = newAttributes;

        ui = newUI;

        state = PlayerState.alive;

        foreach (var i in projectors)
        {
            Material newMaterial = new Material(i.material);
            newMaterial.color = newAttributes.colorPlayer;
            i.material = newMaterial;
        }        

        ui.Set(PlayerUIBox.BoxSetting.alive);
        ui.SetHealth(hpCurrent,hpMax,newAttributes.colorPlayer);

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
            
            if (anim != null)
            {
                anim.SetTrigger("interact");
            }

            int mask = 1 << LayerMask.NameToLayer("Interactable");
            Collider[] cols = Physics.OverlapSphere(transform.position, interactRadius, mask);

            Interactable cachedInteractalbe = null;
            Interactable closest = null;
            float dist = float.MaxValue;
            foreach (var i in cols)
            {
                if (!i.isTrigger && (cachedInteractalbe = i.GetComponentInParent<Interactable>()) != null && cachedInteractalbe != this)
                {
                    float newDist = Vector3.Distance(transform.position, i.transform.position);
                    if (newDist < dist)
                    {
                        closest = cachedInteractalbe;
                        dist = newDist;
                    }
                }
            }

            if (closest != null)   
            {
                Debug.Log("Joy" + indexJoystick.ToString() + "_Player" + indexPlayer.ToString() + " interacted with " + closest.name + ".");
                if (closest is Weapon)  // TODO override current weapon / etc.
                {
                    (closest as Weapon).gameObject.SetLayer(LayerMask.NameToLayer("Player"));
                    AddWeaponToInventory(closest as Weapon);
                    ArrangeUnequippedWeapons();
                }
            }
            else
            {
                Debug.Log("Joy" + indexJoystick.ToString() + "_Player" + indexPlayer.ToString() + " tried to interact, but nothing was found.");
            }

        }        
    }

    public void AttemptRevive()
    {
        if (Time.time > nextRevive)
        {
            nextRevive = Time.time + delayRevive;
            Debug.Log("Joy" + indexJoystick.ToString() + "_Player" + indexPlayer.ToString() + " hit a revive.");   
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
            if (weaponCurrent != null)
            {                
                if (weaponCurrent.AttemptAttack())
                {
                    Debug.Log("Joy" + indexJoystick.ToString() + "_Player" + indexPlayer.ToString() + " attacked with " + weaponCurrent.weaponName + ".");
                }
            }
            else
            {
                // Debug.LogError("No current weapon equipped on P" + indexPlayer);
            }
            if (anim != null)
            {
                anim.SetTrigger("attack");
            }
        }
    }

    public void AttemptAttackAlternate()
    {
        if (Time.time > nextAttackAlternate)
        {
            nextAttackAlternate = Time.time + delayAttackAlternate;
            if (weaponCurrent != null)
            {
                if (weaponCurrent.AttemptAttackAlternate())
                {
                    Debug.Log("Joy" + indexJoystick.ToString() + "_Player" + indexPlayer.ToString() + " alternate attacked with " + weaponCurrent.weaponName + ".");
                }
            }
            else
            {
                Debug.LogError("No current weapon equipped on P" + indexPlayer);
            }
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
            SwapWeapons();
        }        
    }

    void SwapWeapons()
    {
        Debug.Log("Joy" + indexJoystick.ToString() + "_Player" + indexPlayer.ToString() + " swapped weapons.");
        if (anim != null)
        {
            anim.SetTrigger("swapWeapons");
        }

        AddWeaponToInventory(weaponCurrent);

        SetCurrentWeapon(weaponsUnequipped.Dequeue());
        ArrangeUnequippedWeapons();
    }

    public void AttemptThrowWeapon()
    {
        if (Time.time > nextThrowWeapon)
        {
            nextThrowWeapon = Time.time + delayThrowWeapon;
            if (weaponCurrent != null && PlayerManager.instance.prefabBaseWeapon.weaponID != weaponCurrent.weaponID)
            {
                Debug.Log("Joy" + indexJoystick.ToString() + "_Player" + indexPlayer.ToString() + " threw their weapon.");
                if (anim != null)
                {
                    anim.SetTrigger("throwWeapon");
                }
                weaponCurrent.OnDrop();
                weaponCurrent.transform.SetParent(null);
                weaponCurrent.transform.position = positionThrow.position;
                weaponCurrent.rb.AddForce(positionThrow.forward * forceThrow);
                weaponCurrent.gameObject.SetLayer(LayerMask.NameToLayer("Interactable"));

                if (weaponsUnequipped.Count > 0)
                {
                    SwapWeapons();
                }
                else
                {
                    SetCurrentWeapon(PlayerManager.instance.prefabBaseWeapon, true);                    
                }

                if (weaponsUnequipped.Count < PlayerManager.instance.weaponCount - 1)
                {
                    for (int i = 0; i < PlayerManager.instance.weaponCount - (weaponsUnequipped.Count - 1); i++)
                    {
                        AddWeaponToInventory(PlayerManager.instance.prefabBaseWeapon,true);
                    }

                    ArrangeUnequippedWeapons();
                }
            }
        }        
    }

    public void AttemptUseAbility()
    {
        if (Time.time > nextUseAbility)
        {
            nextUseAbility = Time.time + delayUseAbility;
            Debug.Log("Joy" + indexJoystick.ToString() + "_Player" + indexPlayer.ToString() + " used an ability.");   
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

            if (pointsCurrent > 0)
            {
                Debug.Log("Joy" + indexJoystick.ToString() + "_Player" + indexPlayer.ToString() + " threw points.");
                if (anim != null)
                {
                    anim.SetTrigger("throwPoints");
                }

                PooledObject spawnedPointsObject = ObjectPoolingManager.instance.CreateObject(PlayerManager.instance.prefabMoney);
                PickupStats spawnedPoints = spawnedPointsObject.GetComponent<PickupStats>();

                spawnedPoints.type = StatType.points;
                if (pointsCurrent > PlayerManager.instance.pointsToThrow)
                {
                    pointsCurrent -= PlayerManager.instance.pointsToThrow;
                    spawnedPoints.amount = PlayerManager.instance.pointsToThrow;
                }
                else
                {                    
                    spawnedPoints.amount = pointsCurrent;
                    pointsCurrent = 0;
                }

                spawnedPoints.transform.position = positionThrow.position;
                spawnedPoints.rb.AddForce(positionThrow.forward * forceThrow);
            }
        }        
    }

    public void AttemptDisplayMoreInformation()
    {
        if (Time.time > nextDisplayInformation)
        {
            nextDisplayInformation = Time.time + delayDisplayInformation;
            Debug.Log("Joy" + indexJoystick.ToString() + "_Player" + indexPlayer.ToString() + " displayed more HUD information.");

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

    void SetCurrentWeapon(Weapon toSet, bool isPrefab = false)
    {
        if (isPrefab)
        {
            GameObject spawnedWeaponObject = Instantiate(toSet.gameObject);
            Weapon spawnedWeapon = spawnedWeaponObject.GetComponent<Weapon>();
            weaponCurrent = spawnedWeapon;
        }
        else
        {
            weaponCurrent = toSet;
        }
        
        weaponCurrent.transform.SetParent(positionWeaponCurrent);
        weaponCurrent.transform.Reset();
        weaponCurrent.OnEquip(this.gameObject);
    }

    void AddWeaponToInventory(Weapon toAdd, bool isPrefab = false)
    {
        if (toAdd == null)
        {
            return;
        }

        if (isPrefab)
        {
            GameObject spawnedWeaponObject = Instantiate(toAdd.gameObject);
            Weapon spawnedWeapon = spawnedWeaponObject.GetComponent<Weapon>();
            spawnedWeapon.OnUnequip();
            weaponsUnequipped.Enqueue(spawnedWeapon);
        }
        else
        {
            toAdd.OnUnequip();
            weaponsUnequipped.Enqueue(toAdd);
        }
    }

    void ArrangeUnequippedWeapons()
    {
        int index = 0;
        foreach (var i in weaponsUnequipped)
        {
            if (index >= positionsWeaponsUnequipped.Count)
            {
                Debug.LogError("Not enough weapon positions for unequipped weapons on P" + indexPlayer);
                return;
            }
            i.transform.SetParent(positionsWeaponsUnequipped[index]);
            i.transform.Reset();
            index++;
        }
    }
}
