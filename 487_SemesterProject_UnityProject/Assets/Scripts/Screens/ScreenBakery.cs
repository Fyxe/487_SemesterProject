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
        
    }

    public void SelectBasketHolder(HolderBasket newHolder)
    {
        holderCurrent = newHolder;
        textBasketTitle.text = holderCurrent.basket.basketName;
        textBasketCost.text = holderCurrent.basket.basketCost.ToString();
        textBasketDescription.text = holderCurrent.basket.basketDescription;
        // TODO items inside the basket
    }

}
