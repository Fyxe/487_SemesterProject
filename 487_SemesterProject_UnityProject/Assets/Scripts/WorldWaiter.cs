using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class WorldWaiter : Interactable
{
    [Header("Settings")]
    public bool isInteractable = true;
    public bool resetOnInteraction = false;
    public float timeWait = 2f;
    public bool isWaiting = false;
    public bool reverseFill = false;
    public UnityEvent onFinishedWaiting;    

    [Header("References")]
    public Image imageFill;
    public List<Interactable> toInteractWith = new List<Interactable>();

    Coroutine coroutineWait;

    void Awake()
    {
        if (imageFill != null)
        {
            if (reverseFill)
            {
                imageFill.fillAmount = 0f;
            }
            else
            {
                imageFill.fillAmount = 1f;
            }
        }
    }

    public override bool InteractWithPlayer(ControllerMultiPlayer player)
    {        
        return StartWait(player);
    }

    public virtual bool StartWait(ControllerMultiPlayer playerPressedBy)
    {
        if (isInteractable && (resetOnInteraction || (!resetOnInteraction && !isWaiting)))
        {
            if (coroutineWait != null)
            {
                StopCoroutine(coroutineWait);
            }
            coroutineWait = StartCoroutine(Wait(playerPressedBy));
            return true;
        }
        return false;
    }

    IEnumerator Wait(ControllerMultiPlayer playerPressedBy)
    {
        isWaiting = true;
        if (imageFill != null)
        {
            float fillStart = 0f;
            float fillEnd = 1f;

            if (reverseFill)
            {
                fillStart = 1f;
                fillEnd = 0f;
            }

            float currentTime = 0f;
            while (currentTime < timeWait)
            {
                currentTime += Time.deltaTime;
                imageFill.fillAmount = Mathf.Lerp(fillStart, fillEnd, currentTime / timeWait);
                yield return null;
            }
            imageFill.fillAmount = fillEnd;
        }
        else
        {
            yield return new WaitForSeconds(timeWait);
        }
        
        onFinishedWaiting.Invoke();
        foreach (var i in toInteractWith)
        {
            i.InteractWithPlayer(playerPressedBy);
        }
        isWaiting = false;
    }
}
