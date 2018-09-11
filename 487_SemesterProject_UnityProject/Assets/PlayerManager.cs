using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : SingletonDDOL<PlayerManager>
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
        for(int i = 1; i <= 8; i++)
        {
            if (Input.GetKeyDown("joystick " + i + " button 7"))
            {
                Debug.Log("P" + i + " pressed start.");
                AttemptSpawnPlayer(i);
            }
        }
    }

    void AttemptSpawnPlayer(int indexJoystick)
    {
        foreach(var i in allPlayerAttributes)
        {
            if (i.indexJoystick == indexJoystick && i.isSpawned)
            {
                Debug.Log("Player already spawned for joystick P" + indexJoystick);
                return;
            }
        }
        foreach(var i in allPlayerAttributes)
        {
            if (!i.isSpawned)
            {
                i.ResetValues();
                i.isSpawned = true;
                i.indexJoystick = indexJoystick;
                FindObjectOfType<LevelManager>().SpawnPlayer(i);
                return;
            }
        }
        Debug.Log("All players spawned already.");
    }

}
