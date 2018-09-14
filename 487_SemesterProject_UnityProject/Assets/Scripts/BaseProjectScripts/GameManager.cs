using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonDDOL<GameManager>
{

    [Header("Settings")]
    public int currentLevel = 1;

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
                ProgressionManager.instance.SetLevel(data.gameLevelCurrent);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }    
    }

    public void GoToNextLevel()
    {
        currentLevel++;
        if (currentLevel % 2 == 0)
        {
            LoadSceneManager.instance.LoadScene("Shop");
        }
        else
        {
            LoadSceneManager.instance.LoadScene("InLevel");
        }
    }

    public void GoToMainMenu()
    {
        ProgressionManager.instance.OnGameEnd();
        LoadSceneManager.instance.LoadScene("Menu");
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
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    void OnApplicationQuit()
    {
        GameData data = new GameData();
        data.gameLevelCurrent = ProgressionManager.instance.currentGameLevel;
        LocalDataManager.instance.SaveObjectToFile("Saves","GameData.save",data);
    }

}
