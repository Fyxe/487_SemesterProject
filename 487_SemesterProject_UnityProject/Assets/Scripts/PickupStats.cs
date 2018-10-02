using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickupStats : PooledObject
{
    [Header("Settings")]
    public StatType type = StatType.points;
    public int amount = 10;
    public int baseCost;

    ControllerMultiPlayer cachedPlayer;

    [HideInInspector]
    public Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider col)
    {        
        if (!col.isTrigger && (cachedPlayer = col.GetComponentInParent<ControllerMultiPlayer>()) != null)
        {
            switch (type)
            {
                case StatType.points:
                    cachedPlayer.pointsCurrent += amount;
                    break;
                case StatType.healthCurrent:
                    cachedPlayer.hpCurrent += amount;
                    break;
                case StatType.healthMax:
                    cachedPlayer.hpMax += amount;
                    break;
                case StatType.movementSpeed:
                    cachedPlayer.speedMoveCurrent += amount;
                    break;
                case StatType.baseDamage:
                    cachedPlayer.damageBaseCurrent += amount;
                    break;
                case StatType.reviveCount:
                    if (amount > 0)
                    {
                        cachedPlayer.countReviveCurrent *= 2;
                    }
                    else
                    {
                        cachedPlayer.countReviveCurrent /= 2;
                    }
                    break;
                case StatType.stalenessMeter:
                    if (SceneManager.GetActiveScene().name == "InLevel")
                    {
                        (LevelManager.instance as GameLevelManager).currentStaleness += amount;
                    }
                    break;
                default:
                    break;
            }
            DestroyThisObject();
        }
    }

}
