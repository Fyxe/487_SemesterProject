using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UI;

public class ScreenMain : ScreenAnimate
{

    [Header("References")]
    public Text textPoints;
    public Button buttonStartGame;

    public override void OnTransitionedInStart()
    {
        base.OnTransitionedInStart();
        textPoints.text = ProgressionManager.instance.scoreCurrent.ToString();
        EventSystem.current.SetSelectedGameObject(buttonStartGame.gameObject);
    }

}
