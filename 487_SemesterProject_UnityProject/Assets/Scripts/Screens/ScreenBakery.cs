using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;

public class ScreenBakery : ScreenAnimate
{

    public HolderBasket holderCurrent;

    [Header("References")]
    public Text textBasketTitle;
    public Text textBasketCost;
    public Text textBasketDescription;
    public Text textPoints;
    public Transform parentBasket;
    public Button buttonBuy;

    [Header("Prefabs")]
    public GameObject prefabBasket;

    public override void OnTransitionedInStart()
    {
        base.OnTransitionedInStart();
        textPoints.text = ProgressionManager.instance.scoreCurrent.ToString();

        parentBasket.DestroyChildren();
        foreach (var i in ProgressionManager.instance.allBaskets)
        {
            GameObject spawnedHolderObject = Instantiate(prefabBasket, parentBasket);
            HolderBasket spawnedHolder = spawnedHolderObject.GetComponent<HolderBasket>();
            spawnedHolder.Setup(i,this);
            if (holderCurrent == null)
            {
                SelectBasketHolder(spawnedHolder);
            }
        }
        if (holderCurrent == null)
        {
            buttonBuy.interactable = false;
        }
    }

    public void SelectBasketHolder(HolderBasket newHolder)
    {
        holderCurrent = newHolder;
        buttonBuy.interactable = !holderCurrent.isBought;
        textBasketTitle.text = holderCurrent.basket.basketName;
        textBasketCost.text = holderCurrent.basket.basketCost.ToString();
        textBasketDescription.text = holderCurrent.basket.basketDescription;
        // TODO items inside the basket
    }

    public void CallbackBuyCurrentlySelected()
    {
        if (ProgressionManager.instance.scoreCurrent < holderCurrent.basket.basketCost)
        {
            return;
        }
        ProgressionManager.instance.scoreCurrent -= holderCurrent.basket.basketCost;
        ProgressionManager.instance.UnlockBasket(holderCurrent.basket);
        holderCurrent.Setup(holderCurrent.basket,this);
        SelectBasketHolder(holderCurrent);
    }

}
