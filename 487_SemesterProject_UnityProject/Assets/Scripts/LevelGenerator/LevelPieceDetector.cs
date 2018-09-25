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

    public List<ControllerMultiPlayer> m_playersInPiece = new List<ControllerMultiPlayer> ();
    List<ControllerMultiPlayer> playersInPiece
    {
        get
        {
            if (m_playersInPiece.Count > 0 && !(GameLevelManager.instance as GameLevelManager).piecesPlayersAreIn.Contains(myPiece))
            {
                (GameLevelManager.instance as GameLevelManager).AddToPiecesPlayersAreIn(myPiece);
            }
            else if (m_playersInPiece.Count == 0 && (GameLevelManager.instance as GameLevelManager).piecesPlayersAreIn.Contains(myPiece))
            {
                (GameLevelManager.instance as GameLevelManager).RemoveFromPiecesPlayersAreIn(myPiece);
            }
            return m_playersInPiece;
        }
        set
        {
            m_playersInPiece = value;
        }
    }
    LevelPiece myPiece;
    ControllerMultiPlayer cachedPlayer;

    void Awake()
    {
        myPiece = GetComponentInParent<LevelPiece>();    
    }

    public void OnTriggerEnter(Collider col)
    {
        if (!col.isTrigger && (cachedPlayer = col.GetComponentInParent<ControllerMultiPlayer>()) != null && !playersInPiece.Contains(cachedPlayer))
        {            
            playersInPiece.Add(cachedPlayer);
        }
        else
        {
            
        }
    }

    public void OnTriggerExit(Collider col)
    {
        if (!col.isTrigger && (cachedPlayer = col.GetComponentInParent<ControllerMultiPlayer>()) != null && playersInPiece.Contains(cachedPlayer))
        {            
            playersInPiece.Remove(cachedPlayer);
        }
        else
        {
            
        }
    }
}
