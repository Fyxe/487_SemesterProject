using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonDDOL<GameManager>
{
    public GameData dataCurrent = new GameData ();

    protected override void Initialize()
    {

    }

    void Start()
    {
        if (GameManager.instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            GameData data = new GameData();
            if (LocalDataManager.instance.LoadObjectFromFile("Saves", "GameData.save", out data))
            {
                ProgressionManager.instance.scoreCurrent = data.scoreCurrent;
                foreach (var i in data.unlockedBaskets)
                {
                    ProgressionManager.instance.UnlockBasket(i);
                }
            }
            else
            {
                data = new GameData();
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Quit();
        }
    }

    /// <summary>
    /// Sets the time scale to zero.
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Sets the time scale to one.
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Quits the game, or if in editor stops playing. This may need to be changed for different operating systems.
    /// </summary>
    public void Quit()
    {
#if UNITY_EDITOR
        Debug.Log("Closing Editor Application");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OnApplicationQuit()
    {
        LocalDataManager.instance.SaveObjectToFile("Saves","GameData.save",dataCurrent);
    }
}
