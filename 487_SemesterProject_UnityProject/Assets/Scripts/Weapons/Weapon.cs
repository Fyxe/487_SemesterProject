using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Weapon : Interactable
{    
    public class CachedRenderer
    {
        public Renderer renderer;
        List<Material> materials = new List<Material> ();

        public CachedRenderer(Renderer newRenderer)
        {
            renderer = newRenderer;
            materials = renderer.materials.ToList();
        }

        public void SetMaterial(Material material)
        {
            renderer.materials = new Material[1]{ material };
        }

        public void ResetMaterials()
        {
            renderer.materials = materials.ToArray();
        }
    }


    [Header("Weapon Settings")]
    public string weaponName = "?";
    public string weaponDescription = "?";
    public Rarity rarity = Rarity.common;
    public int iDWeapon = -1;
    public int damage = 1;
    public float delayAttack = 1f;
    float nextAttack = 0f;
    public float speedAttack = 1f;
    public float rangeAttack = 2f;
    public int clipCurrent = 10;
    public int clipMax = 10;
    public float timeReload = 2f;
    public bool isReloading = false;

    List<CachedRenderer> cachedRenderers = new List<CachedRenderer>();

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
            cachedRenderers.Add(new CachedRenderer (i));
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

    public void AttemptAttack()
    {
        if (!isReloading && Time.time > nextAttack)
        {
            nextAttack = Time.time + delayAttack;
            Attack();

            clipCurrent--;            
            if (clipCurrent == 0)
            {
                StartCoroutine(Reload());
            }
        }
    }

    protected virtual void Attack()
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
        foreach (var i in cachedRenderers)
        {
            i.SetMaterial(WeaponManager.instance.GetMaterialForRarity(rarity));
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

    public virtual void OnEquip()   // called when changed to current weapon
    {
        rb.isKinematic = true;
    }

    public virtual void OnUnequip() // called when enqueued in unequipped weapons
    {
        rb.isKinematic = true;
    }

    public virtual void OnDrop()    // called when thrown
    {
        rb.isKinematic = false;
    }
}
