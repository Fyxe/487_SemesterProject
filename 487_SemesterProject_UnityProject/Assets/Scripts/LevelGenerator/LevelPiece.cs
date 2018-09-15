using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPiece : MonoBehaviour
{
    public bool hasPlayers
    {
        get
        {
            return detector.hasPlayers;
        }
    }

    [Header("References")]
    public List<BoxCollider> levelColliders = new List<BoxCollider>();
    public List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();
    public float flow = -1f;

    public List<LevelPiece> connectedTo
    {
        get
        {
            List<LevelPiece> retList = new List<LevelPiece>();

            foreach (var i in connectionPoints)
            {
                if (i.attachedTo != null)
                {
                    retList.Add(i.attachedTo.piece);
                }
            }

            return retList;
        }
    }

    LevelPieceDetector detector;

    public void SetFlow(float currentAmount, float increaseAmount)
    {
        flow = currentAmount + increaseAmount;        
        foreach (var i in connectedTo)
        {
            if (i.flow < 0f)
            {
                i.SetFlow(flow,increaseAmount);
            }
        }
    }

    [ContextMenu("Setup Piece")]
    public void Setup()
    {
        levelColliders = GetComponentInChildren<LevelPieceDetector>().GetComponentsInChildren<BoxCollider>().ToList();

        connectionPoints = GetComponentsInChildren<ConnectionPoint>().ToList();
        foreach (var i in connectionPoints)
        {
            i.piece = this;
        }
        detector = GetComponentInChildren<LevelPieceDetector>();
    }

    public void RemoveColliders()  
    {
        foreach (var i in levelColliders)
        {
            i.isTrigger = true;
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
