using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;

public class HolderAbility : MonoBehaviour
{
    [Header("References")]
    public Image imageDisplay;
    public Ability ability;
    ScreenRecipe screenRecpie;

    public void Setup(Ability newAbility, ScreenRecipe newScreenRecipe)
    {
        ability = newAbility;
        screenRecpie = newScreenRecipe;

        if (ability.abilitySprite != null)
        {
            imageDisplay.sprite = ability.abilitySprite;
        }
    }

    public void CallbackSelect()
    {
        screenRecpie.SelectHolderAbility(this);
    }
}
