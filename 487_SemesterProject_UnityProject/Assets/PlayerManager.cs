using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [Header("Settings")]
    public int possableMaxPlayers = 4;

    [Header("References")]
    public List<PlayerAttibutes> allPlayerAttributes = new List<PlayerAttibutes>();

    void Awake()
    {
        for (int i = 0; i < possableMaxPlayers; i++)
        {
            allPlayerAttributes.Add(new PlayerAttibutes());
        }        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var i in Input.GetJoystickNames())
            {
                Debug.Log(i); 
            }
        }
        //for (int i = 0; i < 20; i++)
        //{
        //    if (Input.GetKeyDown("joystick 1 button " + i))
        //    {
        //        print("joystick 1 button " + i);
        //    }
        //}
        for (int i = 1; i <= allPlayerAttributes.Count; i++)
        {            
            if (!allPlayerAttributes[i - 1].isSpawned && Input.GetKeyDown("joystick " + i + " button 7"))
            {
                Debug.Log(i);
                SpawnPlayer(i - 1);
            }
        }
    }

    void SpawnPlayer(int whichToSpawn)
    {
        allPlayerAttributes[whichToSpawn].ResetValues();
        allPlayerAttributes[whichToSpawn].isSpawned = true;
        allPlayerAttributes[whichToSpawn].indexPlayer = whichToSpawn;
        FindObjectOfType<LevelManager>().SpawnPlayer(allPlayerAttributes[whichToSpawn + 1]);
    }

}
