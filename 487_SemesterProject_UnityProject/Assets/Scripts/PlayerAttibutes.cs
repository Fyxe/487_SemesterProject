using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAttributes
{
    public bool isSpawned = false;
    public bool isDead = false;
    public Color colorPlayer = Color.blue;
    public int indexJoystick = 0;
    public int indexPlayer = 0;
    public int pointsCurrent = 0;
    public int hpCurrent = 3;
    public int hpMax = 3;
    public float speedMoveCurrent = 3f;
    public int damageBaseCurrent = 0;
    public int countReviveCurrent = 1;
    public GameObject prefabController;
    public List<int> enemiesKilled = new List<int> ();

    public void ResetValues()
    {
        isSpawned = false;
        isDead = false;
        //colorPlayer = Color.blue;
        indexJoystick = 0;
        indexPlayer = 0;
        pointsCurrent = 0;
        hpCurrent = 3;
        hpMax = 3;
        speedMoveCurrent = 5f;
        damageBaseCurrent = 0;
        countReviveCurrent = 1;
        enemiesKilled = new List<int>();
    }
}
