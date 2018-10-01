using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldButton : Interactable
{
    [Header("Settings")]
    public float delayPress = 0.25f;
    float nextPress = 0f;
    public UnityEvent onPressed;

	public virtual bool PressButton(ControllerMultiPlayer playerPressedBy)
    {
        if (Time.time > nextPress)
        {
            nextPress = Time.time + delayPress;
            onPressed.Invoke();
            return true;
        }
        else
        {
            return false;
        }
    }
}
