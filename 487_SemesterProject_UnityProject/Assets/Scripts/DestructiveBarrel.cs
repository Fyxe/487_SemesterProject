using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructiveBarrel : Damageable
{

    [Header("Settings")]
    public bool overrideLevelDropset;
    public DropSet dropSet;
    public Transform positionDrop;

    public override void OnDeath()
    {
        base.OnDeath();
        isDead = true;

        GameObject spawnedDropObject = null;

        if (overrideLevelDropset)
        {
            spawnedDropObject = Instantiate(dropSet.GetDrop());
        }
        else
        {
            spawnedDropObject = Instantiate(LevelManager.instance.baseDropSetBarrel.GetDrop());
        }
        spawnedDropObject.transform.position = positionDrop.position;
        spawnedDropObject.transform.rotation = positionDrop.rotation;

        // Effects go here

        Destroy(this.gameObject);
    }

}
