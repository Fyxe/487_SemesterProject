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
    public float radiusRevive
    {
        get
        {
            return PlayerManager.instance.radiusRevive;
        }
    }
    public float radiusInteract
    {
        get
        {
            return PlayerManager.instance.radiusInteract;
        }
    }

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
            controllerInput.speedMove = speedMoveCurrent;
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
            if (value == 0)
            {
                attributes.countReviveCurrent = 1;
            }
            else
            {
                attributes.countReviveCurrent = value;
            }
            ui.textReviveCount.text = countReviveCurrent.ToString();
        }
    }    

    int revivesRemaining = 0;

    public int indexJoystick
    {
        get
        {
            return attributes.indexJoystick;
        }
    }
    public int indexPlayer
    {
        get
        {
            return attributes.indexPlayer;
        }
    }

    [HideInInspector] public PlayerAttributes attributes;
    InputController3D controllerInput;
    WeaponController controllerWeapons;
    Animator anim;
    Coroutine coroutineInvulnerable;
    Coroutine coroutineIncapacitate;
    public List<Projector> projectors = new List<Projector>();
    bool triggerInUseRight = false;
    bool triggerInUseLeft = false;
    bool isFalling = false;
    public Rigidbody rb;
    public Collider colliderMovement;
    public bool isInShop = false;

    public bool isControlled
    {
        get
        {
            return LevelManager.instance.isPlaying && !isFalling && !isInShop;
        }

    }
    
    void Awake()
    {
        controllerInput = GetComponent<InputController3D>();      
        controllerWeapons = GetComponent<WeaponController>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {        

        if (!isControlled)
        {
            controllerInput.SetAxis(0f, 0f, 0f, 0f);
            return;
        }

        if (state != PlayerState.alive)
        {            
            ui.imageReviveCount.fillAmount = Mathf.Lerp(ui.imageReviveCount.fillAmount,(float)revivesRemaining / (float)countReviveCurrent,0.2f);
            controllerInput.SetAxis(0f, 0f, 0f, 0f);
            return;
        }


        
        float distance = Vector3.Distance(Camera.main.transform.position, 
            Camera.main.transform.parent.position);
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(0.95f, 0.95f, distance));
        Vector3 botLeft = Camera.main.ViewportToWorldPoint(new Vector3(0.05f, 0.05f, distance));
        
        Vector3 movementAxis0 = new Vector3(Input.GetAxis("J" + indexJoystick.ToString() + "_Axis0Horizontal"), 0f, Input.GetAxis("J" + indexJoystick.ToString() + "_Axis0Vertical"));
        float flipper = 1f;
        if (invertAxis1Z)
        {
            flipper = -1f;
        }
        Vector3 movementAxis1 = new Vector3(Input.GetAxis("J" + indexJoystick.ToString() + "_Axis1Horizontal"), 0f, Input.GetAxis("J" + indexJoystick.ToString() + "_Axis1Vertical") * flipper);
        Vector3 pos = transform.position;

        if (pos.x > topRight.x || pos.z > topRight.z || pos.x < botLeft.x || pos.z < botLeft.z)
        {
            
            float angleTowardsCamera = Vector3.SignedAngle(movementAxis0, transform.position - Camera.main.transform.parent.position, Vector3.up);            
            if (Mathf.Abs(angleTowardsCamera) < 120f)
            {

                movementAxis0 = Vector3.zero;
            }
        }
        
        controllerInput.SetAxis(movementAxis0.x, movementAxis0.z, movementAxis1.x, movementAxis1.z);
        

        if (Mathf.Abs(Input.GetAxis("J" + indexJoystick.ToString() + "_Axis1Horizontal")) + 
            Mathf.Abs(Input.GetAxis("J" + indexJoystick.ToString() + "_Axis1Vertical")) > 0)
        {
            AttemptAttack();
        }

        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 0"))
        {
            AttemptInteract();            
        }
        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 1"))
        {
            
        }
        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 2"))
        {
            AttemptRevive();   
        }
        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 3"))
        {
            AttemptSwapWeapons();
        }
        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 4"))
        {
            //AttemptThrowPoints();
            AttemptThrowWeapon();
        }
        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 5"))
        {
            AttemptSwapWeapons();            
        }

        if (Input.GetAxis("J" + indexJoystick.ToString() + "_ButtonTrigger") > 0)
        {                        
            if (!triggerInUseRight)
            {
                triggerInUseRight = true;
                AttemptAttackAlternate();
            }
        }
        else
        {
            triggerInUseRight = false;
        }


        // Left Trigger
        if (Input.GetAxis("J" + indexJoystick.ToString() + "_ButtonTrigger") < 0)
        {            
            if (!triggerInUseLeft)
            {
                triggerInUseLeft = true;
                AttemptUseAbility();
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
            // pause controlled in player manager, TODO move here?
        }
        if (Input.GetKey("joystick " + indexJoystick.ToString() + " button 8") && Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 9"))
        {
            
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
        ui.textPoints.text = pointsCurrent.ToString("c2");
        state = PlayerState.alive;

        foreach (var i in projectors)
        {
            Material newMaterial = new Material(i.material);
            newMaterial.color = newAttributes.colorPlayer;
            i.material = newMaterial;
        }        

        ui.Set(PlayerUIBox.BoxSetting.alive);
        ui.SetHealth(hpCurrent,hpMax,newAttributes.colorPlayer);

        speedMoveCurrent = speedMoveCurrent;

        foreach (var i in GetComponentsInChildren<Renderer>())
        {
            i.material.color = colorPlayer;
        }

        controllerWeapons.Setup();
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
            Collider[] cols = Physics.OverlapSphere(transform.position, radiusInteract, mask);

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
                //Debug.Log("Joy" + indexJoystick.ToString() + "_Player" + indexPlayer.ToString() + " interacted with " + closest.name + ".");
                if (closest is Weapon)  // TODO override current weapon / etc.
                {
                    Weapon weaponToInteractWith = closest as Weapon;
                    controllerWeapons.PickupWeapon(weaponToInteractWith);
                }
                else if (closest is WorldButton)
                {
                    WorldButton buttonToInteractWith = closest as WorldButton;
                    if (buttonToInteractWith.PressButton(this))
                    {
                        
                    }
                    else
                    {
                        
                    }
                }
                else
                {
                    closest.InteractWithPlayer(this);
                }
            }
            else
            {
                //Debug.Log("Joy" + indexJoystick.ToString() + "_Player" + indexPlayer.ToString() + " tried to interact, but nothing was found.");
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
            Collider[] cols = Physics.OverlapSphere(transform.position, radiusRevive, mask);            

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
                int didRevive = closest.AttemptReviveThisPlayer();
                if (didRevive == 0)
                {
                    pointsCurrent += PointsManager.instance.pointsOnReviveFull;
                }
                else if (didRevive == 1)
                {
                    pointsCurrent += PointsManager.instance.pointsOnReviveHit;
                }
            }
        }        
    }

    public void AttemptAttack()
    {
        if (Time.time > nextAttack)
        {
            nextAttack = Time.time + delayAttack;
            controllerWeapons.AttemptAttack();
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
            controllerWeapons.AttemptAttackAlternate();
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
            controllerWeapons.SwapWeapons();
        }        
    }

    public void AttemptThrowWeapon()
    {
        if (Time.time > nextThrowWeapon && !controllerWeapons.isHoldingBaseWeapon)
        {
            nextThrowWeapon = Time.time + delayThrowWeapon;
            //Debug.Log("Joy" + indexJoystick.ToString() + "_Player" + indexPlayer.ToString() + " threw their weapon.");
            if (anim != null)
            {
                anim.SetTrigger("throwWeapon");
            }
            controllerWeapons.ThrowCurrentWeapon();
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
                if (pointsCurrent > PointsManager.instance.pointsToThrow)
                {
                    pointsCurrent -= PointsManager.instance.pointsToThrow;
                    spawnedPoints.amount = PointsManager.instance.pointsToThrow;
                }
                else
                {                    
                    spawnedPoints.amount = pointsCurrent;
                    pointsCurrent = 0;
                }

                spawnedPoints.transform.position = positionThrow.position;
                spawnedPoints.rb.AddForce(positionThrow.forward * PlayerManager.instance.forceThrow);
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

    IEnumerator Invulerability(float timeInvulnerable)  // TODO update materials to not effect weapons. Cache the renderers
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

    public int AttemptReviveThisPlayer()
    {
        if (state != PlayerState.incapacitated)
        {
            return -1;
        }
        revivesRemaining++;
        if (revivesRemaining >= countReviveCurrent)
        {
            Revive();
            return 0;
        }
        else
        {
            return 1;
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

    public void SetFalling()
    {
        isFalling = true;
        foreach (var i in GetComponentsInChildren<Projector>())
        {
            i.enabled = false;
        }
        rb.constraints = RigidbodyConstraints.None;
        rb.AddTorque(Random.rotation.eulerAngles * 5f);
        controllerInput.enabled = false;
        this.enabled = false;
        colliderMovement.material = PlayerManager.instance.materialBounce;        
    }

    public void SetNotFalling()
    {
        isFalling = false;
        foreach (var i in GetComponentsInChildren<Projector>())
        {
            i.enabled = true;
        }
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        controllerInput.enabled = true;
        this.enabled = true;
        colliderMovement.material = PlayerManager.instance.materialZero;
    }
}
