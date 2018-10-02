using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBuyHealth : MonoBehaviour
{
    [Header("References")]
    public SpawnPosition spawnPosition;

    [Header("Prefabs")]
    public PooledObject prefabSmall;
    public PooledObject prefabMeduim;
    public PooledObject prefabLarge;


    public void SpawnHealth(int whichKind)
    {
        PooledObject spawnedObject = null;
        switch (whichKind)
        {
            case 0:
                spawnedObject = ObjectPoolingManager.instance.CreateObject(prefabSmall);
                break;
            case 1:
                spawnedObject = ObjectPoolingManager.instance.CreateObject(prefabMeduim);
                break;
            case 2:
                spawnedObject = ObjectPoolingManager.instance.CreateObject(prefabLarge);
                break;
        }
        spawnPosition.SpawnObject(spawnedObject.transform);
    }

}
