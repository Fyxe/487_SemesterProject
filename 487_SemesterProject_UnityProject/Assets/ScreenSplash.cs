using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UI;

public class ScreenSplash :  ScreenAnimate
{

    [Header("References")]
    public Button buttonStartGame;

    public override void OnTransitionedInStart()
    {
        base.OnTransitionedInStart();
        EventSystem.current.SetSelectedGameObject(buttonStartGame.gameObject);
    }
}
