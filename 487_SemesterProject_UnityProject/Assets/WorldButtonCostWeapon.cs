using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldButtonCostWeapon : WorldButtonCost
{
    [Header("References")]
    public Text textWeaponName;
    public SpawnPosition positionDisplay;
    public ShopBuyWeapons shop;
    PooledObject prefabWeapon;

    public void Setup(ShopBuyWeapons newShop)    
    {
        shop = newShop;

        PooledObject newPrefabWeapon = DropManager.instance.GetDrop(Thing.weapon).GetComponent<PooledObject>();
        if (newPrefabWeapon == null)
        {
            Destroy(this.gameObject);
            return;
        }
        prefabWeapon = newPrefabWeapon;

        PooledObject spawnedWeaponObject = ObjectPoolingManager.instance.CreateObject(prefabWeapon);
        positionDisplay.SpawnObject(spawnedWeaponObject.transform);

        Weapon spawnedWeapon = spawnedWeaponObject.GetComponent<Weapon>();
        spawnedWeapon.SetToDisplay();
        textWeaponName.text = spawnedWeapon.weaponName;
        cost = spawnedWeapon.cost;
    }

    public override void OnPressed(ControllerMultiPlayer playerPressedBy)
    {
        base.OnPressed(playerPressedBy);
        PooledObject spawnedWeaponObject = ObjectPoolingManager.instance.CreateObject(prefabWeapon);
        shop.spawnPosition.SpawnObject(spawnedWeaponObject.transform);
    }
}
