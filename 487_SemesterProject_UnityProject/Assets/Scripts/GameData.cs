using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int scoreCurrent;
    public int maxDayReached;
    public List<int> unlockedBaskets = new List<int>();
}
