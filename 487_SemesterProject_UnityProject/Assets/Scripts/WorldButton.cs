using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldButton : Interactable
{
    [Header("Button Settings")]
    public bool isPressable = true;
    public float delayPress = 0.25f;
    protected float nextPress = 0f;
    public UnityEvent onPressed;

    [Header("References")]
    public List<Interactable> toInteractWith = new List<Interactable>();

	public virtual bool PressButton(ControllerMultiPlayer playerPressedBy)
    {
        if (isPressable && Time.time > nextPress)
        {
            nextPress = Time.time + delayPress;
            onPressed.Invoke();
            bool retBool = true;
            foreach (var i in toInteractWith)
            {
                if (!i.InteractWithPlayer(playerPressedBy))
                {
                    retBool = false;
                }
            }
            if (retBool)
            {
                OnPressed(playerPressedBy);
            }
            return retBool;
        }
        else
        {
            return false;
        }
    }

    public virtual void OnPressed(ControllerMultiPlayer playerPressedBy)
    {

    }
}
