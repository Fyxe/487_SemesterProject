using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : Singleton<DropManager>
{

    [Header("References")]
    public DropSet dropSetWeapons;
    [HideInInspector] public DropSet dropSetWeaponsInstantiated;

    public DropSet dropSetAbilities;
    [HideInInspector] public DropSet dropSetAbilitiesInstantiated;

    public DropSet dropSetAIs;
    [HideInInspector] public DropSet dropSetAIsInstantiated;

    Weapon cachedWeapon = null;
    Ability cachedAbility = null;
    AI cachedAI = null;

    void Awake()
    {
        if (dropSetWeapons == null)
        {
            Debug.LogError("There is no provided weapon drop set.");
        }
        else
        {
            dropSetWeaponsInstantiated = ScriptableObject.Instantiate(dropSetWeapons);            
        }

        if (dropSetAbilities == null)
        {
            Debug.LogError("There is no provided Ability drop set.");
        }
        else
        {
            dropSetAbilitiesInstantiated = ScriptableObject.Instantiate(dropSetAbilities);
        }

        if (dropSetAIs == null)
        {
            Debug.LogError("There is no provided AI drop set.");
        }
        else
        {
            dropSetAIsInstantiated = ScriptableObject.Instantiate(dropSetAIs);
        }
    }

    public void AddDropSet(DropSet toAdd, Thing type)
    {
        switch (type)
        {
            case Thing.weapon:
                dropSetWeaponsInstantiated.CombineWith(toAdd);
                UpdateUnlockedWeapons();
                break;
            case Thing.ability:
                dropSetAbilitiesInstantiated.CombineWith(toAdd);
                UpdateUnlockedAbilities();
                break;
            case Thing.ai:
                dropSetAIsInstantiated.CombineWith(toAdd);
                UpdateUnlockedAI();
                break;
            case Thing.levelpiece:
                break;
        }
    }

    public GameObject GetDrop(Thing type)
    {
        switch (type)
        {
            case Thing.weapon:
                return dropSetWeaponsInstantiated.GetDrop();
            case Thing.ability:
                return dropSetAbilitiesInstantiated.GetDrop();
            case Thing.ai:
                return dropSetAIsInstantiated.GetDrop();
            case Thing.levelpiece:
                break;
        }
        return null;
    }

    public void UpdateUnlockedWeapons()
    {
        List<Weapon> tempList = new List<Weapon>();
        foreach (var i in dropSetWeaponsInstantiated.allDrops)
        {
            if (((cachedWeapon = i.drop.GetComponent<Weapon>()) != null) && !tempList.Contains(cachedWeapon))
            {
                tempList.Add(cachedWeapon);
            }
        }
        ProgressionManager.instance.allUnlockedWeapons = tempList.OrderBy(x => x.weaponID).ToList();
    }

    public void UpdateUnlockedAbilities()
    {
        List<Ability> tempList = new List<Ability>();
        foreach (var i in dropSetAbilitiesInstantiated.allDrops)
        {
            if (((cachedAbility = i.drop.GetComponent<Ability>()) != null) && !tempList.Contains(cachedAbility))
            {
                tempList.Add(cachedAbility);
            }
        }
        ProgressionManager.instance.allUnlockedAbilities = tempList.OrderBy(x => x.abilityID).ToList();
    }

    public void UpdateUnlockedAI()
    {
        List<AI> tempList = new List<AI>();
        foreach (var i in dropSetAIsInstantiated.allDrops)
        {
            if (((cachedAI = i.drop.GetComponent<AI>()) != null) && !tempList.Contains(cachedAI))
            {
                tempList.Add(cachedAI);
            }
        }
        ProgressionManager.instance.allUnlockedAI = tempList.OrderBy(x => x.aiID).ToList();
    }

}
