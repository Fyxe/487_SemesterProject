using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAttributes
{
    public bool isSpawned = false;
    public bool isDead = false;
    public bool isPriority = false;
    public bool isInvisible = false; // The player can turn invisible. 
    public Color colorPlayer = Color.blue;
    public int indexJoystick = 0;
    public int indexPlayer = 0;
    public int pointsCurrent = 0;
    public int hpCurrent = 3;
    public int hpMax = 3;
    public float speedMoveCurrent = 6f;
    public int damageBaseCurrent = 0;
    public int countReviveCurrent = 1;
    public GameObject prefabController;
    public int abilityCurrentID = -1;
    public int weaponCurrentID = -1;
    public List<int> weaponsUnequippedIDs = new List<int>();
    public List<int> enemiesKilled = new List<int> ();

    public void ResetValues()
    {
        isSpawned = false;
        isDead = false;
        isPriority = false;
        isInvisible = false;
        //colorPlayer = Color.blue;
        indexJoystick = 0;
        indexPlayer = 0;
        pointsCurrent = 0;
        hpCurrent = 3;
        hpMax = 3;
        speedMoveCurrent = 8f;
        damageBaseCurrent = 0;
        countReviveCurrent = 1;
        abilityCurrentID = -1;
        weaponCurrentID = -1;
        weaponsUnequippedIDs = new List<int>();
        enemiesKilled = new List<int>();
    }
}
