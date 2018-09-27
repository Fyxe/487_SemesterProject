using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;

public class HolderAI : MonoBehaviour
{
    [Header("References")]
    public Image imageDisplay;
    public AI ai;
    ScreenRecipe screenRecpie;

    public void Setup(AI newAI, ScreenRecipe newScreenRecipe)
    {
        ai = newAI;
        screenRecpie = newScreenRecipe;

        if (ai.aiSprite != null)
        {
            imageDisplay.sprite = ai.aiSprite;
        }
    }

    public void CallbackSelect()
    {
        screenRecpie.SelectHolderAI(this);
    }
}
