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
        if (GameLevelManager.instance is GameLevelManager)
        {
            (GameLevelManager.instance as GameLevelManager).CallbackEndGame();
        }
        else if (ShopManager.instance is ShopManager)
        {
            GameplayManager.instance.EndGame();
        }
    }

	public void CallbackQuit()
    {
        GameManager.instance.Quit();
    }
}
