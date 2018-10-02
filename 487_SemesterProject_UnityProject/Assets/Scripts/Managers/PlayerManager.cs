using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UI;

public class PlayerManager : SingletonDDOL<PlayerManager>
{

    [Header("Settings")]
    public int possableMaxPlayers = 4;
    public float timeIncapacitated = 10f;
    public float timeInvulnerable = 1f;
    [Tooltip("This does not include the current weapon")]
    public int weaponCount = 1;
    public float forceThrow = 500f;
    public float radiusRevive = 2f;
    public float radiusInteract = 2f;
    public bool replaceCurrentWeaponOnPickup = true;
    public bool swapToNonBaseWeaponOnThrow = true;

    [Header("References")]
    public Sprite spriteHealthFull;
    public Sprite spriteHealthEmpty;
    public PhysicMaterial materialZero;
    public PhysicMaterial materialBounce;

    [Tooltip("This weapon should not appear in the world anywhere.")]
    public Weapon prefabBaseWeapon;
    public PickupStats prefabMoney;
    //[SerializeField]List<PlayerAttributes> m_allPlayerAttributes = new List<PlayerAttributes>();  // TODO make get set for spawned player attributes
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
        //for (int i = 0; i < 20; i++)
        //{
        //    if (Input.GetKeyDown("joystick 1 button " + i))
        //    {
        //        print("joystick 1 button " + i);
        //    }
        //}
        if (SceneManager.GetActiveScene().name == "InLevel" || SceneManager.GetActiveScene().name == "Shop" || SceneManager.GetActiveScene().name.Contains("testing_"))    // TODO fix this
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
                LevelManager.instance.TogglePause();
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
                    //Debug.Log("Player P" + i.indexPlayer + ":J" + i.indexJoystick + " spawned in Level.");
                    GameLevelManager.instance.SpawnPlayer(i);
                }
                else if (ShopManager.instance is ShopManager)
                {
                    //Debug.Log("Player P" + i.indexPlayer + ":J" + i.indexJoystick + " spawned in Shop.");
                    ShopManager.instance.SpawnPlayer(i);
                }
                else if (LevelManager.instance is LevelManager)
                {
                    //Debug.Log("Player P" + i.indexPlayer + ":J" + i.indexJoystick + " spawned in Level.");
                    LevelManager.instance.SpawnPlayer(i);
                }
                return;
            }
        }
        //Debug.Log("All players spawned already.");
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
