using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LevelPieceDetectorHelper : MonoBehaviour
{
    LevelPieceDetector detector;

    void Awake()
    {
        detector = GetComponentInParent<LevelPieceDetector>();    
    }

    void OnTriggerEnter(Collider col)
    {
        detector.OnTriggerEnter(col);
    }

    void OnTriggerExit(Collider col)
    {
        detector.OnTriggerExit(col);
    }
}
