using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{

    // If this controller is on a player it will use the settings in player manager
    [Header("Settings, see comment in code")]
    [SerializeField] float m_forceThrow = 500f;
    public float forceThrow
    {
        get
        {
            if (controllerType == ControllerType.player)
            {
                return PlayerManager.instance.forceThrow;
            }
            else
            {
                return m_forceThrow;
            }
        }
    }
    [SerializeField] int m_weaponCount = 1;
    public int weaponCount
    {
        get
        {
            if (controllerType == ControllerType.player)
            {
                return PlayerManager.instance.weaponCount;
            }
            else
            {
                return m_weaponCount;
            }
        }
    }
    [SerializeField] bool m_replaceCurrentWeaponOnPickup = true;
    public bool replaceCurrentWeaponOnPickup
    {
        get
        {
            if (controllerType == ControllerType.player)
            {
                return PlayerManager.instance.replaceCurrentWeaponOnPickup;
            }
            else
            {
                return m_replaceCurrentWeaponOnPickup;
            }
        }
    }
    [SerializeField] bool m_swapToNonBaseWeaponOnThrow = true;
    public bool swapToNonBaseWeaponOnThrow
    {
        get
        {
            if (controllerType == ControllerType.player)
            {
                return PlayerManager.instance.swapToNonBaseWeaponOnThrow;
            }
            else
            {
                return m_swapToNonBaseWeaponOnThrow;
            }
        }
    }

    [Header("References")]
    public AI attachedAI;
    public ControllerMultiPlayer attachedPlayer;
    [SerializeField] Weapon m_weaponCurrent;
    public Weapon weaponCurrent
    {
        get
        {
            return m_weaponCurrent;
        }
        set
        {
            m_weaponCurrent = value;
            if (controllerType == ControllerType.player)
            {
                if (m_weaponCurrent == null)
                {
                    attachedPlayer.attributes.weaponCurrentID = -1;
                }
                else
                {
                    attachedPlayer.attributes.weaponCurrentID = m_weaponCurrent.weaponID;
                }
            }
        }
    }
    [SerializeField] List<Weapon> m_weaponsUnequipped = new List<Weapon>();
    public List<Weapon> weaponsUnequipped
    {
        get
        {
            if (controllerType == ControllerType.player)
            {
                attachedPlayer.attributes.weaponsUnequippedIDs.Clear();
                foreach (var i in m_weaponsUnequipped)
                {
                    attachedPlayer.attributes.weaponsUnequippedIDs.Add(i.weaponID);
                }
            }
            return m_weaponsUnequipped;
        }
        set
        {
            m_weaponsUnequipped = value;
            if (controllerType == ControllerType.player)
            {
                attachedPlayer.attributes.weaponsUnequippedIDs.Clear();
                foreach (var i in m_weaponsUnequipped)
                {
                    attachedPlayer.attributes.weaponsUnequippedIDs.Add(i.weaponID);
                }
            }
        }
    }
    public Transform positionWeaponCurrent;
    public List<Transform> positionsWeaponsUnequipped = new List<Transform>();
    [SerializeField] Transform m_positionThrow;
    public Transform positionThrow
    {
        get
        {
            if (controllerType == ControllerType.player)
            {
                return attachedPlayer.positionThrow;
            }
            else
            {
                return m_positionThrow;
            }
        }
    }

    ControllerType controllerType = ControllerType.NONE;

    [Header("Prefabs")]
    [SerializeField] Weapon m_prefabBaseWeapon;
    public Weapon prefabBaseWeapon
    {
        get
        {
            if (controllerType == ControllerType.player)
            {
                return PlayerManager.instance.prefabBaseWeapon;
            }
            else
            {
                return m_prefabBaseWeapon;
            }
        }
    }


    public bool hasEmptyWeaponSlotInInventory
    {
        get
        {
            if (isHoldingBaseWeapon)
            {
                return true;
            }
            foreach (var i in weaponsUnequipped)
            {
                if (i.weaponID == prefabBaseWeapon.weaponID)
                {
                    return true;
                }
            }
            return false;
        }
    }
    public bool hasNonEmptyWeaponSlotInInventory
    {
        get
        {
            if (!isHoldingBaseWeapon)
            {
                return true;
            }
            foreach (var i in weaponsUnequipped)
            {
                if (i.weaponID != prefabBaseWeapon.weaponID)
                {
                    return true;
                }
            }
            return false;
        }
    }
    public bool isHoldingBaseWeapon
    {
        get
        {
            return weaponCurrent != null && weaponCurrent.weaponID == prefabBaseWeapon.weaponID;
        }
    }
    public int weaponsInInventory
    {
        get
        {
            int retInt = 0;
            foreach (var i in weaponsUnequipped)
            {
                if (i.weaponID != prefabBaseWeapon.weaponID)
                {
                    retInt++;
                }
            }
            return retInt;
        }
    }

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        attachedAI = GetComponent<AI>();
        attachedPlayer = GetComponent<ControllerMultiPlayer>();

        if (attachedAI != null)
        {
            controllerType = ControllerType.ai;
        }
        if (attachedPlayer != null)
        {
            controllerType = ControllerType.player;
        }
    }

    public void Setup()
    {
        Initialize();
        if (controllerType == ControllerType.player)
        { 
            Weapon spawnCurrent = ProgressionManager.instance.GetWeaponByID(attachedPlayer.attributes.weaponCurrentID, true);
            Debug.Log("ID: "+ attachedPlayer.attributes.weaponCurrentID);
            if (spawnCurrent == null)
            {
                Debug.Log("null");
                SetCurrentWeapon(prefabBaseWeapon, true);
            }
            else
            {
                SetCurrentWeapon(spawnCurrent, true);
            }

            List<int> allIDS = attachedPlayer.attributes.weaponsUnequippedIDs.ToList();
            foreach (var i in allIDS)
            {
                Weapon spawnUnequipped = ProgressionManager.instance.GetWeaponByID(i, true);
                if (spawnUnequipped == null)
                {
                    Debug.Log("spawned prefab");
                    AddWeaponToInventory(prefabBaseWeapon, true);
                }
                else
                {
                    AddWeaponToInventory(spawnUnequipped, true);
                }
            }
        }
        else
        {
            SetCurrentWeapon(prefabBaseWeapon, true);
            for (int i = 0; i < weaponCount; i++)
            {
                AddWeaponToInventory(prefabBaseWeapon, true);
            }
        }
    }

    public bool AttemptAttack()
    {
        if (weaponCurrent == null)
        {
            return false;
        }
        else
        {
            return weaponCurrent.AttemptAttack();
        }
    }

    public bool AttemptAttackAlternate()
    {
        if (weaponCurrent == null)
        {
            return false;
        }
        else
        {
            return weaponCurrent.AttemptAttackAlternate();
        }
    }

    public void PickupWeapon(Weapon toPickup)
    {
        if (replaceCurrentWeaponOnPickup)
        {
            if (isHoldingBaseWeapon)
            {
                weaponCurrent.DestroyThisObject();
            }
            else
            {
                int inventoryCount = weaponsInInventory;
                if (inventoryCount < weaponCount)  
                {
                    AddWeaponToInventory(weaponCurrent);
                    SetCurrentWeapon(toPickup);
                }
                else
                {
                    DropCurrentWeapon();
                }
            }
            SetCurrentWeapon(toPickup);
        }
        else
        {
            AddWeaponToInventory(toPickup);
        }
    }

    public void ThrowCurrentWeapon()
    {
        if (isHoldingBaseWeapon)
        {
            return;
        }
        DropCurrentWeapon();
        if (swapToNonBaseWeaponOnThrow)
        {
            if (weaponsInInventory > 0)
            {
                int index = 0;
                for (int i = 0; i < weaponsUnequipped.Count; i++)
                {
                    if (weaponsUnequipped[i].weaponID != prefabBaseWeapon.weaponID)
                    {
                        index = i;
                        break;
                    }
                }
                Weapon removedWeapon = weaponsUnequipped.RemoveAndGetAt(index);
                SetCurrentWeapon(removedWeapon);
            }
            else
            {
                SetCurrentWeapon(prefabBaseWeapon, true);
            }
        }
        else
        {
            SetCurrentWeapon(prefabBaseWeapon, true);
        }
    }

    public void DropCurrentWeapon()
    {
        weaponCurrent.OnDrop();
        weaponCurrent.transform.SetParent(null);
        weaponCurrent.transform.position = positionThrow.position;
        weaponCurrent.rb.AddForce(positionThrow.forward * forceThrow);
        weaponCurrent.rb.AddTorque(Random.rotation.eulerAngles * forceThrow);

        weaponCurrent = null;
    }

    public void SwapWeapons()
    {
        if (weaponCount == 0)
        {
            return;
        }

        Weapon tempWeapon = weaponCurrent;
        if (tempWeapon == null)
        {
            tempWeapon = ObjectPoolingManager.instance.CreateObject(prefabBaseWeapon) as Weapon;
            tempWeapon.OnEquip(this);
        }
        if (weaponsUnequipped.Count < weaponCount)
        {
            int max = weaponCount - weaponsUnequipped.Count;
            for (int i = 0; i < max; i++)
            {
                Weapon tempBackpackWeapon = ObjectPoolingManager.instance.CreateObject(prefabBaseWeapon) as Weapon;
                tempBackpackWeapon.OnUnequip();
                weaponsUnequipped.Add(tempBackpackWeapon);
            }
        }
        weaponCurrent = weaponsUnequipped.RemoveFirst();
        tempWeapon.OnUnequip();
        weaponsUnequipped.Add(tempWeapon);

        weaponCurrent.OnEquip(this);
        weaponCurrent.transform.SetParent(positionWeaponCurrent);
        weaponCurrent.transform.Reset();

        ArrangeUnequippedWeapons();
    }

    void SetCurrentWeapon(Weapon toSet, bool isPrefab = false)
    {
        if (toSet == null)
        {
            Debug.LogError("Null weapon could not be set.");
            return;
        }

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
        weaponCurrent.OnEquip(this);
    }

    void AddWeaponToInventory(Weapon toAdd, bool isPrefab = false)
    {
        if (toAdd == null)
        {
            Debug.Log("Cannot add null weapon to inventory");
            return;
        }

        if (isPrefab)
        {
            GameObject spawnedWeaponObject = Instantiate(toAdd.gameObject);
            Weapon spawnedWeapon = spawnedWeaponObject.GetComponent<Weapon>();
            spawnedWeapon.OnUnequip();
            weaponsUnequipped.Add(spawnedWeapon);
        }
        else
        {
            bool replaced = false;
            for (int i = 0; i < weaponsUnequipped.Count; i++)
            {
                if (weaponsUnequipped[i].weaponID == prefabBaseWeapon.weaponID)
                {
                    Weapon toDestroy = weaponsUnequipped[i];
                    weaponsUnequipped[i] = toAdd;
                    toDestroy.DestroyThisObject();
                    replaced = true;
                }
            }
            toAdd.OnUnequip();
            if (!replaced)
            {
                weaponsUnequipped.Add(toAdd);
            }
        }
        if (weaponsUnequipped.Count > weaponCount)
        {
            Weapon toDrop = weaponsUnequipped.RemoveFirst();

            toDrop.OnDrop();
            toDrop.transform.SetParent(null);
            toDrop.transform.position = positionThrow.position;
            toDrop.rb.AddForce(positionThrow.forward * forceThrow);
            toDrop.rb.AddTorque(Random.rotation.eulerAngles * forceThrow);
        }
        ArrangeUnequippedWeapons();
    }

    void ArrangeUnequippedWeapons()
    {
        int index = 0;
        foreach (var i in weaponsUnequipped)
        {
            if (index >= positionsWeaponsUnequipped.Count)
            {
                Debug.LogError("Not enough weapon positions for unequipped weapons on P" + attachedPlayer.indexPlayer);
                return;
            }
            i.transform.SetParent(positionsWeaponsUnequipped[index]);
            i.transform.Reset();
            index++;
        }
    }
    
    public bool AddForceToAttachedEntity(Vector3 forceToAdd, ForceMode mode = ForceMode.Force)
    {
        switch (controllerType)
        {
            case ControllerType.ai:
                if (attachedAI.rb.isKinematic)
                {
                    return false;
                }
                else
                {
                    attachedAI.rb.AddForce(forceToAdd, mode);
                    return true;
                }
            case ControllerType.player:
                if (attachedPlayer.rb.isKinematic)
                {
                    return false;
                }
                else
                {
                    attachedPlayer.rb.AddForce(forceToAdd, mode);
                    return true;
                }     
        }
        return false;
    }
}
