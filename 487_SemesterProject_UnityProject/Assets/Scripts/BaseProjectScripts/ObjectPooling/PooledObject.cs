using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PooledObject : MonoBehaviour
{

    [Header("Pooled Object")]
    public bool isWaitingToBeDestroyed = false;
    public ObjectPool pool;
    Coroutine destroyCoroutine;

    /// <summary>
    /// Tells it's corresponding pool to destroy this object.
    /// </summary>
    public void DestroyThisObject()
    {        
        StopAllCoroutines();
        pool.DestroyObject(this);
    }

    /// <summary>
    /// Called when the attached pool destroys this object.
    /// </summary>
    public virtual void OnDestroyedByPool()
    {
        
    }

    /// <summary>
    /// Called when the attached pool spawns this object.
    /// </summary>
    public virtual void OnCreatedByPool()
    {

    }

    /// <summary>
    /// Calls the coroutine that calls 'DestroyThisObject' after a given time delay.
    /// </summary>
    /// <param name="time">The amount of time in seconds before this object is destroyed.</param>
    public void DestroyAfterTime(float time)
    {
        if (isWaitingToBeDestroyed)
        {
            StopCoroutine(destroyCoroutine);
        }
        destroyCoroutine = StartCoroutine(DestroyAfterTimeCoroutine(time));
    }

    /// <summary>
    /// Calls 'DestroyThisObject' after a given time delay.
    /// </summary>
    /// <param name="time">The amount of time in seconds before this object is destroyed.</param>
    IEnumerator DestroyAfterTimeCoroutine(float time)
    {
        isWaitingToBeDestroyed = true;
        yield return new WaitForSeconds(time);
        DestroyThisObject();
        isWaitingToBeDestroyed = false;
    }
}
