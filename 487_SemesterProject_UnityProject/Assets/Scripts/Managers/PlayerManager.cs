﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : SingletonDDOL<PlayerManager>
{

    [Header("Settings")]
    public int possableMaxPlayers = 4;

    [Header("References")]
    public List<PlayerAttributes> allPlayerAttributes = new List<PlayerAttributes>();

    public int playersInGame
    {
        get 
        {
            int retInt = 0;
            foreach(var i in allPlayerAttributes)
            {
                if (i.isSpawned)
                {
                    retInt++;
                }
            }
            return retInt;
        }
    }

    void Awake()
    {
        for (int i = 0; i < possableMaxPlayers; i++)
        {
            allPlayerAttributes.Add(new PlayerAttributes());
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
        if (SceneManager.GetActiveScene().name == "InLevel" || SceneManager.GetActiveScene().name == "Shop")
        {
            for (int i = 1; i <= 8; i++)
            {
                if (Input.GetKeyDown("joystick " + i + " button 7"))
                {
                    Debug.Log("P" + i + " pressed start.");
                    AttemptSpawnPlayer(i);
                }
            }
        }        
    }

    void AttemptSpawnPlayer(int indexJoystick)
    {
        foreach(var i in allPlayerAttributes)
        {
            if (i.indexJoystick == indexJoystick && i.isSpawned)
            {
                //Debug.Log("Player already spawned for joystick P" + indexJoystick);
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
                i.indexPlayer = allPlayerAttributes.IndexOf(i);
                
                if (ShopManager.instance != null)
                {
                    Debug.Log("Player P" + i.indexPlayer + " spawned in Shop.");
                    ShopManager.instance.SpawnPlayer(i);
                }
                else if (LevelManager.instance != null)
                {
                    Debug.Log("Player P" + i.indexPlayer + " spawned in Level.");
                    LevelManager.instance.SpawnPlayer(i);
                }
                
                return;
            }
        }
        Debug.Log("All players spawned already.");
    }

    public PlayerAttributes GetAttributeOfPlayer(int playerIndex)
    {
        foreach (var i in allPlayerAttributes)
        {
            if (i.indexPlayer == playerIndex)
            {
                return i;
            }
        }
        return null;
    }

    public void ResetAllPlayers()
    {
        foreach (var i in allPlayerAttributes)
        {
            i.ResetValues();
        }
    }
}