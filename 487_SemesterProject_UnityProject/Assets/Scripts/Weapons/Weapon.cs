using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Weapon : Interactable
{
    // TODO create a weapon controller that has reference to the player object, then move all scripts from the controllermultiplayer over
    

    [Header("Weapon Settings")]
    public int weaponID = -1;
    public string weaponName = "?";
    public string weaponDescription = "?";
    public Rarity rarity = Rarity.common;
    public Sprite weaponSprite;
    public int baseCost = 0;
    public int upgradeID = -1;
    [Space]
    public int damage = 1;
    public float delayAttack = 1f;
    float nextAttack = 0f;
    public float delayAttackAlternate = 1f;
    float nextAttackAlternate = 0f;
    public float speedAttack = 1f;  // TODO rename
    public float rangeAttack = 2f;
    public int clipCurrent = 10;
    [Tooltip("Set this to -1 to never reload.")]
    public int clipMax = 10;
    public float timeReload = 2f;
    public bool isReloading = false;
    bool isHeld = false;
    public WeaponController controllerCurrent;

    public int cost
    {
        get
        {
            return (PointsManager.instance.pointsPerWeaponLevel * (int)rarity) + baseCost;
        }
    }

    List<CachedRenderer> cachedRenderers = new List<CachedRenderer>();

    [HideInInspector]
    public Rigidbody rb;

    public PlayerChecker checkerHighlight;
    public PlayerChecker checkerInformation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var i in renderers)
        {
            cachedRenderers.Add(new CachedRenderer(i));
        }
        checkerHighlight.Setup(DisableHighlight,EnableHighlight, WeaponManager.instance.rangeHighlight);
        checkerInformation.Setup(DisableInformation, EnableInformation, WeaponManager.instance.rangeInformation);        
        Initialize();
    }

    protected virtual void Initialize()
    {

    }

    public virtual bool AttemptAttack(int damageBase, float damageMultipier, float attackRateMultiplier)
    {
        if (!isReloading && Time.time > nextAttack)
        {
            nextAttack = Time.time + (delayAttack / attackRateMultiplier);
            Attack(damageBase, damageMultipier);

            if (clipMax != -1)
            {
                clipCurrent--;
                if (clipCurrent == 0)
                {
                    StartCoroutine(Reload());
                }
            }
            return true;
        }
        return false;
    }

    protected virtual void Attack(int damageBase, float damageMultipier)
    {

    }

    public virtual bool AttemptAttackAlternate()
    {
        if (!isReloading && Time.time > nextAttack)
        {
            nextAttackAlternate = Time.time + delayAttack;
            AttackAlternate();

            if (clipMax != -1)
            {
                clipCurrent--;
                if (clipCurrent == 0)
                {
                    StartCoroutine(Reload());
                }
            }
            return true;
        }
        return false;
    }

    protected virtual void AttackAlternate()
    {

    }

    protected virtual IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(timeReload);

        clipCurrent = clipMax;

        isReloading = false;
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

    public virtual void OnEquip(WeaponController newControllerCurrent)   // called when changed to current weapon
    {
        rb.isKinematic = true;
        controllerCurrent = newControllerCurrent;
        isHeld = true;
        DisableHighlight();
        this.gameObject.ReplaceLayer(LayerMask.NameToLayer("Uninteractable"), LayerMask.NameToLayer("Interactable"));
    }

    public virtual void OnUnequip() // called when enqueued in unequipped weapons
    {
        rb.isKinematic = true;
        controllerCurrent = null;
        isHeld = true;
        DisableHighlight();
        this.gameObject.ReplaceLayer(LayerMask.NameToLayer("Interactable"), LayerMask.NameToLayer("Uninteractable"));
    }

    public virtual void OnDrop()    // called when thrown
    {
        rb.isKinematic = false;
        isHeld = false;
        checkerHighlight.playersInRange.Add(controllerCurrent.attachedPlayer);
        checkerInformation.playersInRange.Add(controllerCurrent.attachedPlayer);
        EnableHighlight();
        EnableInformation();
        this.gameObject.ReplaceLayer(LayerMask.NameToLayer("Interactable"), LayerMask.NameToLayer("Uninteractable"));
    }

    public virtual void OnKill(Damageable damageable)
    {
        Debug.Log("Killed dameagable: " + damageable.name);
    }

    public virtual void OnAttack()
    {
        // Gets called whenever you attack, even if that attack results in a kill
    }

    public virtual void SetToDisplay()
    {
        this.gameObject.SetLayer("Default");
        rb.isKinematic = true;
    }
}
