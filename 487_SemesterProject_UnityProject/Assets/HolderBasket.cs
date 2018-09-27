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
        }
        else
        {
            imageDisplay.color = Color.black;
        }
    }

    public void CallbackSelect()
    {
        screenBakery.SelectBasketHolder(this);
    }
	
}
