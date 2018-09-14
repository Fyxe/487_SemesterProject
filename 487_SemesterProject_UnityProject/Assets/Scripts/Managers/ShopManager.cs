using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopManager : LevelManager
{

    void Start()
    {        
        foreach (var i in playerUIBoxes)
        {
            i.Set(PlayerUIBox.BoxSetting.empty);
        }
        StartLevel();
    }
}
