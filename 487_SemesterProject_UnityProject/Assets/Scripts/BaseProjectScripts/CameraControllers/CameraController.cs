using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CameraController : MonoBehaviour {

    [Header("Camera Controller")]
    public float lerpSpeed = 0.2f;
    public List<Transform> currentTargets = new List<Transform>();

    /// <summary>
    /// Caluclates the center of the targets.
    /// </summary>
    /// <returns>The center point of all target's positions.</returns>
    public Vector3 GetCenterOfTargets()
    {
        Vector3 retVec = Vector3.zero;
        foreach (Transform i in currentTargets)
        {
            retVec += i.position;
        }
        retVec /= currentTargets.Count;
        return retVec;
    }
}
