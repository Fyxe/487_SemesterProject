using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ability : Interactable
{

    [Header("Ability Settings")]
    public int abilityID = -1;
    public string abilityName = "?";
    public string abilityDescription = "?";
    public Rarity rarity = Rarity.common;
    public Sprite abilitySprite;
    public int baseCost = 0;
    [Space]
    public float durationAttack = 1f;
    public float delayAttack = 1f;
    float nextAttack = 0f;
    [Space]
    public float delayAttackAlternate = 1f;
    float nextAttackAlternate = 0f;

    public bool isInUse = false;
    bool isHeld = false;
    public AbilityController controllerCurrent;
    public int cost
    {
        get
        {
            return (PointsManager.instance.pointsPerAbilityLevel * (int)rarity) + baseCost;
        }
    }

    public Rigidbody currentPlayerRB;

    List<Weapon.CachedRenderer> cachedRenderers = new List<Weapon.CachedRenderer>();

    [HideInInspector]
    public Rigidbody rb;

    [Header("References")]
    public GameObject model;
    public PlayerChecker checkerHighlight;
    public PlayerChecker checkerInformation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var i in renderers)
        {
            cachedRenderers.Add(new Weapon.CachedRenderer(i));
        }
        checkerHighlight.Setup(DisableHighlight, EnableHighlight, WeaponManager.instance.rangeHighlight);
        checkerInformation.Setup(DisableInformation, EnableInformation, WeaponManager.instance.rangeInformation);        
        Initialize();
    }

    protected virtual void Initialize()
    {

    }

    public virtual bool AttemptAttack()
    {
        if (Time.time > nextAttack)
        {
            nextAttack = Time.time + delayAttack;
                        
            return Attack();
        }
        return false;
    }

    protected virtual bool Attack()
    {
        return true;
    }

    public virtual bool AttemptAttackAlternate()
    {
        if (Time.time > nextAttack)
        {
            nextAttack = Time.time + delayAttack;
            AttackAlternate();
            
            return true;
        }
        return false;
    }

    protected virtual void AttackAlternate()
    {

    }

    public void EnableHighlight()
    {
        if (!isHeld)
        {
            foreach (var i in cachedRenderers)
            {
                i.SetMaterial(WeaponManager.instance.GetMaterialForRarity(rarity));
            }
        }
    }

    public void DisableHighlight()
    {
        foreach (var i in cachedRenderers)
        {
            i.ResetMaterials();
        }
    }

    public void EnableInformation()
    {
        // todo
    }

    public void DisableInformation()
    {
        // todo
    }

    public virtual void OnEquip(AbilityController newController)   // called when changed to current weapon
    {
        rb.isKinematic = true;
        controllerCurrent = newController;
        isHeld = true;
        DisableHighlight();
        ActivateModel();
    }

    public virtual void OnUnequip() // called when enqueued in unequipped weapons
    {
        rb.isKinematic = true;
        controllerCurrent = null;
        isHeld = true;
        DisableHighlight();
    }

    public virtual void OnDrop()    // called when thrown
    {
        rb.isKinematic = false;
        isHeld = false;
        DeactivateModel();
    }

    public virtual void ActivateModel()
    {
        model.SetActive(true);
    }

    public virtual void DeactivateModel()
    {
        model.SetActive(false);
    }
}
