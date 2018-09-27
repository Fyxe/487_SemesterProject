using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;

public class HolderWeapon : MonoBehaviour
{
    [Header("References")]
    public Image imageDisplay;
    public Weapon weapon;
    ScreenRecipe screenRecpie;

    public void Setup(Weapon newWeapon, ScreenRecipe newScreenRecipe)
    {
        weapon = newWeapon;
        screenRecpie = newScreenRecipe;

        if (weapon.weaponSprite != null)
        {
            imageDisplay.sprite = weapon.weaponSprite;
        }
    }

    public void CallbackSelect()
    {
        screenRecpie.SelectHolderWeapon(this);
    }
}
