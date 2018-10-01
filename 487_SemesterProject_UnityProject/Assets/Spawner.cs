using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : Interactable
{
    [Header("Settings")]
    [Tooltip("Set this to -1 to infinitly spawn objects")]
    public int maxToSpawn = -1;
    public int currentAmountSpawned = 0;    

    [Header("References")]
    public PooledObject prefabToSpawn;
    public SpawnPosition spawnPosition;

    public override bool InteractWithPlayer(ControllerMultiPlayer player)
    {
        if (maxToSpawn == -1 || (maxToSpawn != -1 && currentAmountSpawned < maxToSpawn))
        {
            Debug.Log("spawned object");
            PooledObject spawnedObject = ObjectPoolingManager.instance.CreateObject(prefabToSpawn);
            spawnPosition.SpawnObject(spawnedObject.transform);
            return true;
        }
        Debug.Log("spawned no object");
        return false;
    }

}
