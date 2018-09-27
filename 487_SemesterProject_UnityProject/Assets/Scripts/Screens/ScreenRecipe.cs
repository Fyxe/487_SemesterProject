using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;

public class ScreenRecipe : ScreenAnimate
{
    public HolderWeapon holderWeaponCurrent;
    public HolderAbility holderAbilityCurrent;
    public HolderAI holderAICurrent;

    [Header("References")]
    public Text textTitle;    
    public Text textDescription;
    [Space]
    public Transform parentWeapon;
    public Transform parentAbility;
    public Transform parentAI;
    List<HolderWeapon> allHolderWeapons = new List<HolderWeapon>();
    List<HolderAbility> allHolderAbilities = new List<HolderAbility>();
    List<HolderAI> allHolderAIs = new List<HolderAI>();

    [Header("Prefabs")]
    public GameObject prefabWeapon;
    public GameObject prefabAbility;
    public GameObject prefabAI;

    public override void OnTransitionedInStart()
    {
        base.OnTransitionedInStart();

        parentWeapon.DestroyChildren();
        parentAbility.DestroyChildren();
        parentAI.DestroyChildren();

        foreach (var i in ProgressionManager.instance.allUnlockedWeapons)
        {
            GameObject spawnedWeaponHolderObject = Instantiate(prefabWeapon, parentWeapon);
            HolderWeapon spawnedWeaponHolder = spawnedWeaponHolderObject.GetComponent<HolderWeapon>();
            spawnedWeaponHolder.Setup(i, this);
            if (holderWeaponCurrent == null)
            {
                SelectHolderWeapon(spawnedWeaponHolder);
            }
            allHolderWeapons.Add(spawnedWeaponHolder);
        }

        foreach (var i in ProgressionManager.instance.allUnlockedAbilities)
        {
            GameObject spawnedAbilityHolderObject = Instantiate(prefabAbility, parentAbility);
            HolderAbility spawnedAbilityHolder = spawnedAbilityHolderObject.GetComponent<HolderAbility>();
            spawnedAbilityHolder.Setup(i, this);
            if (holderAbilityCurrent == null)
            {
                SelectHolderAbility(spawnedAbilityHolder);
            }
            allHolderAbilities.Add(spawnedAbilityHolder);
        }

        foreach (var i in ProgressionManager.instance.allUnlockedAI)
        {
            GameObject spawnedAIHolderObject = Instantiate(prefabAI, parentAI);
            HolderAI spawnedAIHolder = spawnedAIHolderObject.GetComponent<HolderAI>();
            spawnedAIHolder.Setup(i, this);
            if (holderAICurrent == null)
            {
                SelectHolderAI(spawnedAIHolder);
            }
            allHolderAIs.Add(spawnedAIHolder);
        }
    }

    public void SelectHolderWeapon(HolderWeapon newHolder)
    {
        holderWeaponCurrent = newHolder;
        textTitle.text = holderWeaponCurrent.weapon.weaponName;
        textDescription.text = holderWeaponCurrent.weapon.weaponDescription;        
    }

    public void SelectHolderAbility(HolderAbility newHolder)
    {
        holderAbilityCurrent = newHolder;
        textTitle.text = holderAbilityCurrent.ability.abilityName;
        textDescription.text = holderAbilityCurrent.ability.abilityDescription;
    }

    public void SelectHolderAI(HolderAI newHolder)
    {
        holderAICurrent = newHolder;
        textTitle.text = holderAICurrent.ai.aiName;
        textDescription.text = holderAICurrent.ai.aiDescription;
    }

    public void TabOpened(Thing thing)
    {
        switch (thing)
        {
            case Thing.weapon:
                if (allHolderWeapons.Count > 0)
                {
                    SelectHolderWeapon(allHolderWeapons[0]);
                }
                break;
            case Thing.ability:
                if (allHolderAbilities.Count > 0)
                {
                    SelectHolderAbility(allHolderAbilities[0]);
                }
                break;
            case Thing.ai:
                if (allHolderAIs.Count > 0)
                {
                    SelectHolderAI(allHolderAIs[0]);
                }
                break;
        }
    }
}
