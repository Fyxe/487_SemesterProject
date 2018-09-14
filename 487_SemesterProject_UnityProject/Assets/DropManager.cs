using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : Singleton<DropManager>
{

    [Header("References")]
    public DropSet dropSetBase;
    DropSet dropSetMaster;
    List<int> dropSetsAdded = new List<int>();

    void Awake()
    {
        if (dropSetBase == null)
        {
            Debug.LogError("There is no provided base drop set. No drops will be enabled this game.");
        }
        else
        {
            dropSetMaster = ScriptableObject.Instantiate(dropSetBase);
            dropSetsAdded.Add(dropSetBase.idDropSet);
        }
    }

    public bool AddDropSetToMaster(DropSet toAdd)
    {
        if (dropSetsAdded.Contains(toAdd.idDropSet))
        {
            return false;
        }
        else
        {
            dropSetMaster.allDrops.AddRange(toAdd.allDrops);
            dropSetsAdded.Add(toAdd.idDropSet);
            return true;
        }        
    }

    public bool AddDropSetsToMaster(List<DropSet> toAdd)
    {
        bool retBool = true;
        foreach (var i in toAdd)
        {
            if (!AddDropSetToMaster(i))
            {
                retBool = false;
            }
        }
        return retBool;
    }

    public GameObject GetDrop()
    {
        return dropSetMaster.GetDrop();
    }
}
