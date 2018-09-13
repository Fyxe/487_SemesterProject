using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupPoints : PooledObject
{

    [Header("Settings")]
    public int pointsGivenOnPickup = 10;

    ControllerMultiPlayer cachedPlayer;

    void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.name);
        if (!col.isTrigger && (cachedPlayer = col.GetComponentInParent<ControllerMultiPlayer>()) != null)
        {
            cachedPlayer.pointsCurrent += pointsGivenOnPickup;
            DestroyThisObject();
        }
    }

}
