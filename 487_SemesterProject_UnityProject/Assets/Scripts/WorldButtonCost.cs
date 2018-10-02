using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class WorldButtonCost : WorldButton
{
    [Header("Cost Settings")]
    int m_cost = 0;
    public virtual int cost
    {
        get
        {
            return m_cost;
        }
        set
        {
            m_cost = value;
        }
    }
    public bool subtractPoints = true;
    public float costColorTime = 1f;
    public Color colorBase = Color.white;
    public Color colorPressed = Color.green;
    public Color colorNotPressed = Color.red;

    [Header("References")]
    public Text textCost;
    public PlayerChecker checker;

    Coroutine coroutineColorText;

    void Awake()
    {
        if (textCost != null)
        {
            textCost.text = "$" + cost.ToString();
            textCost.color = colorBase;
        }
        if (checker != null)
        {
            checker.playersEmpty += HideCost;
            checker.playersUnempty += ShowCost;
            HideCost();
        }
    }

    void ShowCost()
    {
        if (textCost != null)
        {
            textCost.gameObject.SetActive(true);
        }
    }

    void HideCost()
    {
        if (textCost != null)
        {
            textCost.gameObject.SetActive(false);
        }
    }

    public override bool PressButton(ControllerMultiPlayer playerPressedBy)
    {
        if (isPressable && playerPressedBy.pointsCurrent >= cost && Time.time > nextPress)
        {            
            foreach (var i in toInteractWith)
            {
                if (!i.InteractWithPlayer(playerPressedBy))
                {
                    return false;
                }
            }
            if (subtractPoints)
            {
                playerPressedBy.pointsCurrent -= cost;
            }
            nextPress = Time.time + delayPress;
            onPressed.Invoke();
            if (textCost != null)
            {
                if (coroutineColorText != null)
                {
                    StopCoroutine(coroutineColorText);
                }
                coroutineColorText = StartCoroutine(ColorText(true));
            }
            
            return true;            
        }
        else
        {
            if (textCost != null && playerPressedBy.pointsCurrent < cost)
            {
                if (coroutineColorText != null)
                {
                    StopCoroutine(coroutineColorText);
                }
                coroutineColorText = StartCoroutine(ColorText(false));
            }
            return false;
        }
    }

    IEnumerator ColorText(bool wasPressed)
    {
        if (wasPressed)
        {
            textCost.color = colorPressed;
        }
        else
        {
            textCost.color = colorNotPressed;
        }
        yield return new WaitForSeconds(costColorTime);
        textCost.color = colorBase;
    }
}
