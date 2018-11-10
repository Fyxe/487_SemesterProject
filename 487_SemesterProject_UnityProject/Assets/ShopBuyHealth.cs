using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBuyHealth : MonoBehaviour
{
    [Header("References")]
    public SpawnPosition spawnPosition;
    public AudioClip buyItemSound;

    [Header("Prefabs")]
    public PooledObject prefabSmall;
    public PooledObject prefabMeduim;
    public PooledObject prefabLarge;
    public WorldButtonCost buttonSmall;
    public WorldButtonCost buttonMedium;
    public WorldButtonCost buttonLarge;

    void Start()
    {
        buttonSmall.cost = PointsManager.instance.pointsPerInGameLevelHealth * 1 * GameplayManager.instance.currentLevel;
        buttonMedium.cost = PointsManager.instance.pointsPerInGameLevelHealth * 2 * GameplayManager.instance.currentLevel; 
        buttonLarge.cost = PointsManager.instance.pointsPerInGameLevelHealth * 3 * GameplayManager.instance.currentLevel; 
    }

    public void SpawnHealth(int whichKind)
    {
        PooledObject spawnedObject = null;
        AudioManager.instance.PlayClipLocalSpace(buyItemSound);
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
