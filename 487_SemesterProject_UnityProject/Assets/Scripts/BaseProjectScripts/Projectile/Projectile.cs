using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PooledObject
{
    [Header("Projectile Settings")]
    public bool pierce = true;
    public bool bounce = false;
    public bool showTrail = true;
    [Space]
    public float speedMove = 1f;
    public Vector3 direction;
    public LayerMask layerMask;
    public TrailRenderer trail;
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
            if (!LevelManager.instance.isPlaying)
            {
                yield return null;
                continue;
            }
            if (!CheckForHit())
            {
                UpdatePosition();
                yield return null;
            }
            else if (pierce)
            {
                yield return null;
            }
            else if (bounce)
            {
                // TODO but in bounce mechanics here
                yield return null;           
            }
            else
            {
                DestroyThisObject();
                break;
            }
        }
        yield break;
    }

    bool CheckForHit()
    {
        r = new Ray(transform.position,direction.normalized);
        h = default(RaycastHit);

        // int mask = 1 << layerMask.value; Use?
        if (Physics.Raycast(r, out h, speedMove * Time.deltaTime, layerMask.value))
        {
            if (!h.collider.isTrigger)
            {
                OnHit(h.collider);
                return true;
            }   
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    void UpdatePosition()
    {
        newPosition = transform.position + (direction.normalized * (speedMove * Time.deltaTime));
        if (showTrail)
        {
            Debug.DrawLine(transform.position,newPosition,Color.yellow,0.2f);
        }
        transform.position = newPosition;
    }

    public virtual void OnHit(Collider hit)
    {

    }

    public override void OnDestroyedByPool()
    {
        base.OnDestroyedByPool();
        trail.Clear();
    }

}
