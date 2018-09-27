using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshCameraController : Singleton<NavMeshCameraController> 
{

    [Header("Settings")]
    public float speedLerp = 0.1f;
    public float samplePositionDistance = 10f;
    public float cameraHeightMin = 10;
    public float cameraHeightMax = 20;

    [Header("References")]
    public List<Transform> toFollow = new List<Transform>();
    
    Vector3 target;
    Vector3 positionLast;
    // TODO remember last legal point and if cant find nav mesh point, use that

    void LateUpdate()
    {
        if (toFollow.Count == 0)
        {
            return;
        }

        NavMeshPath path = new NavMeshPath ();
        Vector3 centralPoint = GetCentralPoint();
        NavMeshHit h = new NavMeshHit ();
        int mask = 1 << NavMesh.GetAreaFromName("CameraWalkable");
        if (NavMesh.SamplePosition(centralPoint, out h, samplePositionDistance, mask))
        {
            Debug.DrawLine(centralPoint, centralPoint + Vector3.up, Color.yellow);

            Debug.DrawLine(h.position, h.position + Vector3.up, Color.red);

            target = h.position;
            target.y = transform.position.y;

            transform.position = Vector3.Slerp(transform.position, target, speedLerp);

            positionLast = target;
        }
        else
        {
            transform.position = Vector3.Slerp(transform.position, positionLast, speedLerp);
            Debug.DrawLine(centralPoint, centralPoint + Vector3.up, Color.yellow);            
        }
        
    }

    public Vector3 GetCentralPoint()    // use standard deviation for camera height
    {
        Vector3 retVec = Vector3.zero;
        foreach (Transform i in toFollow)
        {
            retVec += i.position;
        }
        retVec /= toFollow.Count;
        return retVec;
    }
}
