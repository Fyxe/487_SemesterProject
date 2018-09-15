using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerationManager : Singleton<LevelGenerationManager>
{

    [Header("Settings")]
    public LayerMask layerMaskLevelCollision;
    public GenerationType type;
    public bool useWidthMatching;
    public float maxWidthDifference = 0.05f;
    public bool useConnectionTypes;
    public int countToSpawnMin;
    public int countToSpawnMax;
    // public int maxFails;
    public int specialPiecesMin;
    public int specialPiecesMax;
    public float flowIncreaseAmount = 100f;

    [Header("References")]
    public Transform startPosition;
    public List<LevelPiece> piecesGeneral = new List<LevelPiece>();
    public List<LevelPiece> piecesSpecial = new List<LevelPiece>();
    public List<LevelPiece> piecesStart = new List<LevelPiece>();
    public List<LevelPiece> piecesEnd = new List<LevelPiece>();

    [Space]
    public List<LevelPiece> allSpawnedPieces = new List<LevelPiece>();
    public List<LevelPiece> piecesSpawnedOrder = new List<LevelPiece>();
    
    [ContextMenu("Generate Level Depth First")]
    public void GenerateLevelDFS()
    {
        allSpawnedPieces = new List<LevelPiece>();
        piecesSpawnedOrder = new List<LevelPiece>();

        GameObject spawnedStartPieceObject = Instantiate(piecesStart.GetRandomValue().gameObject);
        spawnedStartPieceObject.name += " StartPiece";
        spawnedStartPieceObject.transform.position = startPosition.position;
        LevelPiece spawnedStartPiece = spawnedStartPieceObject.GetComponent<LevelPiece>();
        spawnedStartPiece.Setup();
        piecesSpawnedOrder.Add(spawnedStartPiece);
        allSpawnedPieces.Add(spawnedStartPiece);
        int countToSpawn = Random.Range(countToSpawnMin,countToSpawnMax) - 2;   // -2 for end and start piece
        int originalCountToSpawn = countToSpawn + 2;
        while (countToSpawn != 0)
        {            
            if (AddPieceDFS(piecesGeneral))
            {                
                countToSpawn--;
            }
            else
            {
                piecesSpawnedOrder.RemoveAt(piecesSpawnedOrder.Count - 1);  // todo check has open connections

                if (piecesSpawnedOrder.Count == 0)
                {                    
                    return;
                }
            }
        }
        while (!AddPieceDFS(piecesEnd))
        {

        }

        if (allSpawnedPieces.Count == originalCountToSpawn)
        {
            Debug.Log("Level Generated Successfully");
        }
        else
        {
            Debug.Log("Level failed to generate.");
        }

        piecesSpawnedOrder = new List<LevelPiece>();
        spawnedStartPiece.SetFlow(0f, flowIncreaseAmount);
    }

    bool AddPieceDFS(List<LevelPiece> toSpawn)
    {

        List<LevelPiece> piecesLeftToTry = toSpawn.ToList();
        piecesLeftToTry.Shuffle();
        
        List<ConnectionPoint> myPointsToTry = piecesSpawnedOrder[piecesSpawnedOrder.Count - 1].connectionPoints;
        myPointsToTry.Shuffle();
        foreach (var i in piecesLeftToTry)
        {

            GameObject spawnedPieceObject = Instantiate(i.gameObject);
            LevelPiece spawnedPiece = spawnedPieceObject.GetComponent<LevelPiece>();
            spawnedPiece.Setup();

            List<ConnectionPoint> theirPointsToTry = spawnedPiece.connectionPoints;
            theirPointsToTry.Shuffle();

            foreach (var j in myPointsToTry)
            {
                if (j.attachedTo != null)
                {
                    continue;
                }
                foreach (var k in theirPointsToTry)
                {
                    //if (k.attachedTo != null)
                    //{
                    //    continue;
                    //}

                    if (useWidthMatching && Mathf.Abs(j.width - k.width) > maxWidthDifference)
                    {
                        continue;
                    }

                    if (useConnectionTypes && (!j.connectableTypes.Contains(k.type) || !k.connectableTypes.Contains(j.type)))
                    {
                        continue;
                    }

                    spawnedPieceObject.transform.position = j.transform.position + (k.transform.localPosition * (-1f));
                    
                    float angleToRotate = Vector3.SignedAngle(j.direction, k.direction * (-1f), Vector3.up);                    
                    spawnedPiece.transform.RotateAround(j.transform.position, Vector3.up, -angleToRotate);


                    if (spawnedPiece.FitsInPosition())
                    {
                        allSpawnedPieces.Add(spawnedPiece);
                        piecesSpawnedOrder.Add(spawnedPiece);
                        j.Attach(k);                     
                        return true;
                    }
                }
            }

            Destroy(spawnedPieceObject);
        }
        return false;
    }
}
