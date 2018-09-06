using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPool {

    public PooledObject objectPrefab;    
    public List<PooledObject> deactivatedObjects = new List<PooledObject>();      

    public ObjectPool(PooledObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("ObjectPool was given a null object to create!");
        }
        objectPrefab = prefab;
    }

    /// <summary>
    /// If there are no deactivated objects, creates a new one, otherwise it grabs the last object added to the list.
    /// </summary>    
    /// <param name="startingParent">If the 'created' object's parent.</param>
    /// <param name="destroyTime">The given lifetime that the object should stay alive. Set to (-1) if there should be no destroy time.</param>
    /// <returns>Returns the object that was created.</returns>
    public PooledObject CreateObject(Transform startingParent = null, float destroyTime = -1)
    {
        PooledObject retObj = null;
        if (deactivatedObjects.Count == 0)
        {
            retObj = ObjectPoolingManager.instance.InstantiateObject(objectPrefab);
        }
        else
        {
            retObj = deactivatedObjects[deactivatedObjects.Count - 1];
            deactivatedObjects.RemoveAt(deactivatedObjects.Count - 1);
            retObj.gameObject.SetActive(true);
        }
        retObj.transform.parent = startingParent;
        retObj.transform.Reset();
        retObj.pool = this;
        retObj.OnCreatedByPool();
        if (destroyTime != -1)
        {
            retObj.DestroyAfterTime(destroyTime);
        }        
        return retObj;
    }


    /// <summary>
    /// 'Destroys' the given pooled object. Calls OnDestroyedByPool of the object, 
    /// sets it inactive, then adds it to the list of deactivated objects.
    /// </summary>
    /// <param name="toDestroy">The pooled object that you want to be destroyed.</param>
    public void DestroyObject(PooledObject toDestroy)
    {
        toDestroy.OnDestroyedByPool();
        toDestroy.gameObject.SetActive(false);
        toDestroy.transform.parent = null;
        deactivatedObjects.Add(toDestroy);
    }
}
