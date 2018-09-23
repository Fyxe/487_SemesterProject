using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : SingletonDDOL<PlayerManager>
{

    [Header("Settings")]
    public int possableMaxPlayers = 4;
    public float timeIncapacitated = 10f;
    public float timeInvulnerable = 1f;
    public int weaponCount = 2;
    public int pointsToThrow = 100;

    [Header("References")]
    public Weapon prefabBaseWeapon;
    public PickupStats prefabMoney;
    public List<PlayerAttributes> allPlayerAttributes = new List<PlayerAttributes>();

    public int playersInGame
    {
        get 
        {
            int retInt = 0;
            foreach(var i in allPlayerAttributes)
            {
                if (i.isSpawned && !i.isDead)
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
            foreach (var i in FindObjectsOfType<ControllerMultiPlayer>())
            {
                i.Hurt(1);
            }
        }
        //for (int i = 0; i < 20; i++)
        //{
        //    if (Input.GetKeyDown("joystick 1 button " + i))
        //    {
        //        print("joystick 1 button " + i);
        //    }
        //}
        if (SceneManager.GetActiveScene().name == "InLevel" || SceneManager.GetActiveScene().name == "Shop" || SceneManager.GetActiveScene().name == "testing_AI")
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
        Debug.Log(indexJoystick);
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

                if (GameLevelManager.instance is GameLevelManager)
                {
                    Debug.Log("Player P" + i.indexPlayer + " spawned in Level.");
                    GameLevelManager.instance.SpawnPlayer(i);
                }
                if (ShopManager.instance is ShopManager)
                {
                    Debug.Log("Player P" + i.indexPlayer + " spawned in Shop.");
                    ShopManager.instance.SpawnPlayer(i);
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
