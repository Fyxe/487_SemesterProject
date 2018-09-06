using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPoolingManager : Singleton<ObjectPoolingManager>
{

    [Header("Pooling Manager")]
    public bool ifMaxUseFirstCreated = false;   // TODO - nonfunctional
    public int defaultMaxInPool = -1;           // TODO - nonfunctional
    public List<ObjectPool> allObjectPools = new List<ObjectPool>();

    protected override void Initialize()
    {
        SceneManager.sceneLoaded += OnSceneLoadedReset;
    }

    /// <summary>
    /// Clears all of the information in the pools 
    /// </summary>
    /// <param name="scene">The scene being loaded in.</param>
    /// <param name="mode">The mode that the scene was loaded by.</param>
    void OnSceneLoadedReset(Scene scene, LoadSceneMode mode)
    {
        ClearAllPools();
    }

    /// <summary>
    /// Removes all instances of the pools from the pooling manager.
    /// </summary>
    void ClearAllPools()
    {
        allObjectPools.Clear();
    }

    /// <summary>
    /// Given a prefab, find the pool that is associated with it, or creates a new one.
    /// </summary>
    /// <param name="pooledObjectPrefab">The prefab that should be associated with a pool.</param>
    /// <returns>Returns the pool that the prefab associates with, or makes a new one for that prefab and returns it.</returns>
    ObjectPool GetPoolFromPooledObjectPrefab(PooledObject pooledObjectPrefab)
    {
        ObjectPool retPool = null;
        foreach (ObjectPool i in allObjectPools)
        {
            if (i.objectPrefab == pooledObjectPrefab)
            {
                retPool = i;
                break;
            }
        }

        if (retPool == null)
        {
            retPool = new ObjectPool(pooledObjectPrefab);
            allObjectPools.Add(retPool);
        }

        return retPool;
    }

    /// <summary>
    /// Attempts to create and object and return it.
    /// </summary>
    /// <param name="pooledObjectPrefab">The prefab of the gameobject that you are trying to create an instance of.</param>
    /// <param name="startingParent">If the 'created' object's parent.</param>
    /// <param name="destroyTime">The given lifetime that the object should stay alive. Set to (-1) if there should be no destroy time.</param>
    /// <returns>The object that was created.</returns>
    public PooledObject CreateObject(PooledObject pooledObjectPrefab, Transform newParent = null, float destroyTime = -1)
    {
        if (pooledObjectPrefab == null)
        {
            Debug.Log("Pooling manager was given a null prefab");
            return null;
        }

        PooledObject retObj;
        retObj = GetPoolFromPooledObjectPrefab(pooledObjectPrefab).CreateObject(newParent, destroyTime);
        return retObj;
    }

    /// <summary>
    /// Uses MonoBehaviours Instantiate to create a new gameobject. This is used in ObjectPool
    /// </summary>
    /// <param name="prefab">The prefab to be created.</param>
    /// <returns>Returns the prefab created.</returns>
    public PooledObject InstantiateObject(PooledObject prefab)
    {
        return Instantiate(prefab);
    }

}
