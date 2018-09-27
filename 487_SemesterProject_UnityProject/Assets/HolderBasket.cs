using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;

public class HolderBasket : MonoBehaviour
{

    [Header("References")]
    public Image imageDisplay;
    public BreadBasket basket;
    public bool isBought;
    ScreenBakery screenBakery;

    public void Setup(BreadBasket newBasket, ScreenBakery newScreenBakery)
    {
        basket = newBasket;
        screenBakery = newScreenBakery;

        if (basket.basketSprite != null)
        {
            imageDisplay.sprite = basket.basketSprite;
        }

        if (GameManager.instance.dataCurrent.unlockedBaskets.Contains(newBasket.basketID))
        {
            imageDisplay.color = Color.white;
            isBought = true;
        }
        else
        {
            imageDisplay.color = Color.black;
            isBought = false;
        }
    }

    public void CallbackSelect()
    {
        screenBakery.SelectBasketHolder(this);
    }
	
}
