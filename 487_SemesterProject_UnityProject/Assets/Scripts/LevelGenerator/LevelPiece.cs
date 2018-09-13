using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPiece : MonoBehaviour
{

    [Header("References")]
    public List<BoxCollider> levelColliders = new List<BoxCollider>();
    public List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();

    public void Setup()
    {
        Debug.Log(name);
        levelColliders = GetComponentsInChildren<BoxCollider>().ToList();
        connectionPoints = GetComponentsInChildren<ConnectionPoint>().ToList();
    }

    public void RemoveColliders()   // TODO
    {
        foreach (var i in levelColliders)
        {
            i.enabled = false;
        }
    }
    
    public bool FitsInPosition()
    {
        foreach (var i in levelColliders)
        {
            Collider[] collisions = Physics.OverlapBox(i.transform.position, i.transform.localScale * 0.5f, i.transform.rotation,LevelGenerationManager.instance.layerMaskLevelCollision);            
            foreach (var j in collisions)
            {                
                if (j.GetComponentInParent<LevelPiece>() != this)
                {
                    return false;
                }
            }
        }
        return true;
    }

    [ContextMenu("Check if piece fits")]
    public bool FitsInPositionDebug()
    {
        bool retBool = FitsInPosition();
        Debug.Log(retBool);
        return retBool;
    }

}
