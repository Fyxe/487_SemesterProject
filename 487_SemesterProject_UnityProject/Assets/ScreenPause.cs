using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UI;

public class ScreenPause : ScreenAnimate
{
    [Header("References")]
    public Button resumeButton;

    public override void OnTransitionedInStart()
    {
        base.OnTransitionedInStart();
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }

    public void CallbackResume()
    {
        LevelManager.instance.Resume();
    }

    public void CallbackGoToMenu()
    {
        (GameLevelManager.instance as GameLevelManager).CallbackEndGame();
    }

	public void CallbackQuit()
    {
        GameManager.instance.Quit();
    }
}
