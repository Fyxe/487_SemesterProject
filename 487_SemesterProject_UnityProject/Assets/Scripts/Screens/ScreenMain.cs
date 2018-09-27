using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;

public class ScreenMain : ScreenAnimate
{

    [Header("References")]
    public Text textPoints;

    public override void OnTransitionedInStart()
    {
        base.OnTransitionedInStart();
        textPoints.text = ProgressionManager.instance.scoreCurrent.ToString();
    }

}
