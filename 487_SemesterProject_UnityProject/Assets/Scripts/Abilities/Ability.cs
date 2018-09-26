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
    [Space]
    public float delayAttack = 1f;
    float nextAttack = 0f;
    public float delayAttackAlternate = 1f;
    float nextAttackAlternate = 0f;

    bool isHeld = false;
    GameObject m_currentPlayer;
    public GameObject currentPlayer
    {
        get
        {
            return m_currentPlayer;
        }
        set
        {
            m_currentPlayer = value;
            if (value != null)
            {
                if (m_currentPlayer.GetComponent<Rigidbody>() == null)
                {
                    Debug.LogError("No rigidbody found on player: " + value.name);
                }
                else
                {
                    currentPlayerRB = m_currentPlayer.GetComponent<Rigidbody>();
                }
            }
        }
    }

    public Rigidbody currentPlayerRB;

    List<Weapon.CachedRenderer> cachedRenderers = new List<Weapon.CachedRenderer>();

    [HideInInspector]
    public Rigidbody rb;

    WeaponDetectorHighlight detectorHighlight;
    WeaponDetectorInformation detectorInformation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var i in renderers)
        {
            cachedRenderers.Add(new Weapon.CachedRenderer(i));
        }
        detectorHighlight = GetComponentInChildren<WeaponDetectorHighlight>();
        detectorInformation = GetComponentInChildren<WeaponDetectorInformation>();
        UpdateDetectors();
        Initialize();
    }

    protected virtual void Initialize()
    {

    }

    public void UpdateDetectors()
    {
        detectorHighlight.SetRange(WeaponManager.instance.rangeHighlight);
        detectorInformation.SetRange(WeaponManager.instance.rangeInformation);
    }

    public virtual bool AttemptAttack()
    {
        if (Time.time > nextAttack)
        {
            nextAttack = Time.time + delayAttack;
            Attack();
            
            return true;
        }
        return false;
    }

    protected virtual void Attack()
    {

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

    public virtual void OnEquip(GameObject player)   // called when changed to current weapon
    {
        rb.isKinematic = true;
        currentPlayer = player;
        isHeld = true;
        DisableHighlight();
    }

    public virtual void OnUnequip() // called when enqueued in unequipped weapons
    {
        rb.isKinematic = true;
        currentPlayer = null;
        isHeld = true;
        DisableHighlight();
    }

    public virtual void OnDrop()    // called when thrown
    {
        rb.isKinematic = false;
        isHeld = false;
    }
}
