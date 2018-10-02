using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsManager : Singleton<PointsManager>
{
    [Header("Settings")]
    public int pointsOnReviveHit = 10;
    public int pointsOnReviveFull = 100;
    [Space]
    public int pointsOnEnemyHit = 10;
    public int pointsOnEnemyKill = 50;
    [Space]
    public int pointsPerLevelSpeedMove = 200;
    public int pointsPerLevelBaseDamage = 200;
    public int pointsPerLevelMaxHP = 200;
    [Space]
    public int pointsToThrow = 100;
    [Space]
    public int pointsPerWeaponLevel = 200;

}
