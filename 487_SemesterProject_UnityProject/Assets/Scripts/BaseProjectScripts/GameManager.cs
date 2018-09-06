using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonDDOL<GameManager>
{

    //[Header("GameManager")]

    protected override void Initialize()
    {
                
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
