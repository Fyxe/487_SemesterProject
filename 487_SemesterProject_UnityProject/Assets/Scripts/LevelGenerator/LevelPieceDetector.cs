using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPieceDetector : MonoBehaviour
{
    public bool hasPlayers
    {
        get
        {
            return playersInPiece.Count > 0;
        }
    }

    List<ControllerMultiPlayer> playersInPiece = new List<ControllerMultiPlayer>();
    ControllerMultiPlayer cachedPlayer;
    
    public void OnTriggerEnter(Collider col)
    {
        if (!col.isTrigger && (cachedPlayer = col.GetComponentInParent<ControllerMultiPlayer>()) != null && !playersInPiece.Contains(cachedPlayer))
        {

        }
    }

    public void OnTriggerExit(Collider col)
    {
        if (!col.isTrigger && (cachedPlayer = col.GetComponentInParent<ControllerMultiPlayer>()) != null && playersInPiece.Contains(cachedPlayer))
        {

        }
    }
}
