using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UI;

public class ScreenMain : ScreenAnimate
{

    [Header("References")]
    public Text textMaxDay;
    public Button buttonStartGame;

    public override void OnTransitionedInStart()
    {
        base.OnTransitionedInStart();
        textMaxDay.text = GameManager.instance.dataCurrent.maxDayReached.ToString();
        EventSystem.current.SetSelectedGameObject(buttonStartGame.gameObject);
    }

}
