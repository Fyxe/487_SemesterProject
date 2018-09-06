using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCamera : CameraController {

    [Header("Top Down Camera")]
    public Vector3 offset;
    public bool lookAtTarget = false;

    void LateUpdate()
    {
        Vector3 centerPoint = GetCenterOfTargets();
        transform.position = Vector3.Lerp(transform.position,centerPoint + offset,lerpSpeed);
        
        if (lookAtTarget)
        {
            if (offset.normalized == Vector3.up)
            {
                Debug.LogWarning("CURRENT BUG, will spaz out camera if not offset on the Z axis.");
            }
            transform.LookAt(centerPoint);
        }        
    }    
}
