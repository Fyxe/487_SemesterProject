using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopBuyWeapons : MonoBehaviour
{
    [Header("References")]
    public SpawnPosition spawnPosition;
    public List<SpawnPosition> positionsButtons = new List<SpawnPosition>();

    [Header("Prefabs")]
    public PooledObject prefabButton;

    void Start()
    {
        foreach (var i in positionsButtons)
        {
            PooledObject spawnedButton = ObjectPoolingManager.instance.CreateObject(prefabButton);
            i.SpawnObject(spawnedButton.transform);
            spawnedButton.GetComponent<WorldButtonCostWeapon>().Setup(this);
        }    
    }

}
