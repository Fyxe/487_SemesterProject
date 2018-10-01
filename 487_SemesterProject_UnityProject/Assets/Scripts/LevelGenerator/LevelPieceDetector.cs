using System.Linq;
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
                Debug.Log("Added " + myPiece.name);
                (GameLevelManager.instance as GameLevelManager).AddToPiecesPlayersAreIn(myPiece);
            }
            else if (m_playersInPiece.Count == 0 && (GameLevelManager.instance as GameLevelManager).piecesPlayersAreIn.Contains(myPiece))
            {
                Debug.Log("Removed " + myPiece.name);
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
    List<LevelPieceDetectorHelper> helpers = new List<LevelPieceDetectorHelper>();

    void Awake()
    {
        myPiece = GetComponentInParent<LevelPiece>();
        helpers = GetComponentsInChildren<LevelPieceDetectorHelper>().ToList();
    }

    public void OnTriggerEnter(Collider col)
    {
        if (!col.isTrigger && (cachedPlayer = col.GetComponentInParent<ControllerMultiPlayer>()) != null && !playersInPiece.Contains(cachedPlayer))
        {                        
            playersInPiece.Add(cachedPlayer);
        }
    }

    public void OnTriggerExit(Collider col)
    {
        if (!col.isTrigger && (cachedPlayer = col.GetComponentInParent<ControllerMultiPlayer>()) != null && playersInPiece.Contains(cachedPlayer))
        {            
            //if (!CheckForPlayerInHelpers(cachedPlayer))
            //{
                playersInPiece.Remove(cachedPlayer);
            //}
        }
    }

    public bool CheckForPlayerInHelpers(ControllerMultiPlayer player)
    {
        foreach (var i in helpers)
        {
            if (i.CheckForPlayer(player))
            {
                return true;
            }
        }
        return false;
    }
}
