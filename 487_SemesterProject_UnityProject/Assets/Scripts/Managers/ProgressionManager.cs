using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager : Singleton<ProgressionManager>
{

    [Header("Settings")]
    public int scoreOnLevelCompletion = 1000;

    [Header("References")]
    public List<BreadBasket> allBaskets = new List<BreadBasket>();

    // TODO sorted lists?
    public List<Weapon> allWeapons = new List<Weapon>();            // automatically updated on start of the game by iterating through all available baskets 
    public List<Weapon> allUnlockedWeapons = new List<Weapon>();    // automatically updated in drop manager when baskets are added

    public List<Ability> allAbilities = new List<Ability>();            
    public List<Ability> allUnlockedAbilities = new List<Ability>();

    public List<AI> allAI = new List<AI>();
    public List<AI> allUnlockedAI = new List<AI>();
    
    public int scoreCurrent = 0;
    public int scoreCurrentInLevel = 0;

    Weapon cachedWeapon;
    Ability cachedAbility;
    AI cachedAI;

    void Awake()
    {
        allBaskets = allBaskets.OrderBy(x => x.basketID).ToList();

        List<Weapon> tempListWeapon = new List<Weapon>();
        List<Ability> tempListAbility = new List<Ability>();
        List<AI> tempListAI = new List<AI>();
        foreach (var i in allBaskets)
        {
            foreach (var j in i.dropSetWeapons.allDrops)
            {
                if (((cachedWeapon = j.drop.GetComponent<Weapon>()) != null) && !tempListWeapon.Contains(cachedWeapon))
                {
                    tempListWeapon.Add(cachedWeapon);
                }
            }
            foreach (var j in i.dropSetAbilities.allDrops)
            {
                if (((cachedAbility = j.drop.GetComponent<Ability>()) != null) && !tempListAbility.Contains(cachedAbility))
                {
                    tempListAbility.Add(cachedAbility);
                }
            }
            foreach (var j in i.dropSetAIs.allDrops)
            {
                if (((cachedAI = j.drop.GetComponent<AI>()) != null) && !tempListAI.Contains(cachedAI))
                {
                    tempListAI.Add(cachedAI);
                }
            }
        }
        allWeapons = tempListWeapon.OrderBy(x => x.weaponID).ToList();        
        allAbilities = tempListAbility.OrderBy(x => x.abilityID).ToList();
        allAI = tempListAI.OrderBy(x => x.aiID).ToList();
    }

    public void OnGameEnd()
    {
        scoreCurrent += scoreCurrentInLevel;
        scoreCurrentInLevel = 0;
    }

    public BreadBasket GetBasketByBasketID(int basketID)
    {
        foreach (var i in allBaskets)
        {
            if (i.basketID == basketID)
            {
                return i;
            }
        }
        return null;
    }

    public void UnlockBasket(int basketID)
    {
        BreadBasket basket = GetBasketByBasketID(basketID);
        DropManager.instance.AddDropSet(basket.dropSetWeapons,Thing.weapon);
        DropManager.instance.AddDropSet(basket.dropSetAbilities, Thing.ability);
        DropManager.instance.AddDropSet(basket.dropSetAIs, Thing.ai);
    }

    public void UnlockBasket(BreadBasket basket)
    {        
        DropManager.instance.AddDropSet(basket.dropSetWeapons, Thing.weapon);
        DropManager.instance.AddDropSet(basket.dropSetAbilities, Thing.ability);
        DropManager.instance.AddDropSet(basket.dropSetAIs, Thing.ai);
    }
}
