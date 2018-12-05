using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public void CallbackStartGame()
    {
        GameplayManager.instance.StartGame();
    }

    public void CallbackQuitGame()
    {
        GameManager.instance.Quit();
    }

    public void CallbackPlayButtonClick()
    {
        GameManager.instance.PlayButtonClick();
    }
}
