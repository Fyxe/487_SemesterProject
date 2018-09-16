using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PooledObject
{
    [Header("Projectile Settings")]
    public bool showTrail = true;
    [Space]
    public float speedMove = 1f;
    public bool destroyOnHit = true;
    public Vector3 direction;
    public LayerMask layerMask;

    Ray r;
    RaycastHit h;

    Vector3 newPosition;

    Coroutine coroutineShot;

    public void ShootProjectile(Vector3 newDirection, float newSpeedMove, LayerMask newLayerMask)
    {
        direction = newDirection;
        speedMove = newSpeedMove;
        newLayerMask = layerMask;

        if (coroutineShot != null)
        {
            StopCoroutine(coroutineShot);            
        }

        coroutineShot = StartCoroutine(ProjectilePath());
    }

    IEnumerator ProjectilePath()
    {
        while(true)
        {
            if (!CheckForHit())
            {
                UpdatePosition();
                yield return null;
            }
            else if (destroyOnHit)
            {
                yield break;
            }            
            // TODO limit time alive
        }        
    }

    bool CheckForHit()
    {
        r = new Ray(transform.position,direction);
        h = default(RaycastHit);

        // int mask = 1 << layerMask.value; Use?
        if (Physics.Raycast(r, out h, speedMove, layerMask.value))
        {
            OnHit(h.collider);
            return true;
        }
        else
        {
            return false;
        }
    }

    void UpdatePosition()
    {
        newPosition = transform.position + (direction * (speedMove * Time.deltaTime));
        if (showTrail)
        {
            Debug.DrawLine(transform.position,newPosition,Color.yellow,0.2f);
        }
        transform.position = newPosition;
    }

    public virtual void OnHit(Collider hit)
    {

    }
	
}
