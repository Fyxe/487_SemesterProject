using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LevelPieceDetectorHelper : MonoBehaviour
{
    LevelPieceDetector detector;

    BoxCollider boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
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

    public bool CheckForPlayer(ControllerMultiPlayer player)
    {
        Collider[] cols = Physics.OverlapBox(transform.position, transform.localScale * 0.5f, transform.rotation, LayerMask.NameToLayer("Player"));
        foreach (var i in cols)
        {
            if (i.transform.root == player.transform.root)
            {
                return true;
            }
        }
        return false;
    }
}
