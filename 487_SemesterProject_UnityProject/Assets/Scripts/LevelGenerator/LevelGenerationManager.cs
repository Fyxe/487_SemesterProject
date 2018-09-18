using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerationManager : Singleton<LevelGenerationManager>
{

    [Header("Settings")]
    public LayerMask layerMaskLevelCollision;
    public GenerationType type = GenerationType.DFS;
    public bool useWidthMatching = false;
    public float maxWidthDifference = 0.05f;
    public bool useConnectionTypes = false;
    public int countToSpawnMin = 5;
    public int countToSpawnMax = 5;
    public int maxFails = 3;
    public int specialPiecesMin = 0;
    public int specialPiecesMax = 0;
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
    List<LevelPiece> piecesToAddTo = new List<LevelPiece>();
    
    public void GenerateLevel()
    {
        switch (type)
        {
            case GenerationType.BFS:
                GenerateLevelBFS();
                break;
            case GenerationType.DFS:
                GenerateLevelDFS(null, Random.Range(countToSpawnMin, countToSpawnMax));
                break;
            case GenerationType.Synthesis:
                GenerateLevelCombination();
                break;
            default:
                break;
        }
    }

    [ContextMenu("Generate Level Combination")]
    public bool GenerateLevelCombination()
    {
        int attempt = 0;
        while (attempt < maxFails)
        {
            DestroyAllLevelPieces();
            return true;
        }
        return false;
    }

    [ContextMenu("Generate Level Breath First")]
    public bool GenerateLevelBFS()
    {
        int attempt = 0;
        while (attempt < maxFails)
        {
            DestroyAllLevelPieces();
            return true;
        }
        return false;
    }

    [ContextMenu("Generate Level Depth First")]
    public bool GenerateLevelDFS(LevelPiece toAddOnto, int amountToPlace)
    {
        List<LevelPiece> placedPieces = new List<LevelPiece>();
        List<LevelPiece> placedPiecesOrder = new List<LevelPiece>();        

        int attempt = 0;
        while (attempt < maxFails)
        {
            int placedPiecesCount = placedPieces.Count;
            for (int i = placedPiecesCount - 1; i >= 0; i--)
            {
                Destroy(placedPieces[i].gameObject);
            }
            placedPieces.Clear();
            placedPiecesOrder.Clear();

            LevelPiece currentPiece = null;
            if (toAddOnto == null)
            {
                GameObject spawnedStartPieceObject = Instantiate(piecesStart.GetRandomValue().gameObject);
                spawnedStartPieceObject.name += " StartPiece";
                spawnedStartPieceObject.transform.position = startPosition.position;

                LevelPiece spawnedStartPiece = spawnedStartPieceObject.GetComponent<LevelPiece>();
                spawnedStartPiece.Setup();

                placedPieces.Add(spawnedStartPiece);
                placedPiecesOrder.Add(spawnedStartPiece);

                currentPiece = spawnedStartPiece;
            }
            else
            {
                currentPiece = toAddOnto;
            }            
             
            while (placedPieces.Count != amountToPlace)
            {
                List<LevelPiece> piecesToUse = new List<LevelPiece>();
                if (placedPieces.Count == 0)
                {
                    piecesToUse = piecesStart;
                }
                else if (placedPieces.Count == amountToPlace)
                {
                    piecesToUse = piecesEnd;
                }
                else
                {
                    piecesToUse = piecesGeneral;
                }

                LevelPiece addedPiece = AddPieceDFS(piecesGeneral,currentPiece);
                if (addedPiece != null)
                {                    
                    currentPiece = addedPiece;
                    placedPiecesOrder.Add(addedPiece);
                    placedPieces.Add(addedPiece);
                }
                else
                {
                    if (piecesSpawnedOrder.Count == 0)
                    {
                        break;
                    }

                    placedPiecesOrder.RemoveAt(placedPiecesOrder.Count - 1);  // todo check has open connections

                    if (piecesSpawnedOrder.Count == 0)
                    {
                        break;
                    }
                }
            }

            if (placedPieces.Count == amountToPlace)
            {
                Debug.Log("Level Generated Successfully on attempt " + attempt.ToString() + ".");                                
                return true;
            }
            else
            {
                Debug.Log("Level failed to generate on attempt " + attempt.ToString() + ".");
                attempt++;
            }

        }
        Debug.Log("Level failed to generate after " + attempt.ToString() + " attempts.");
        return false;
    }

    LevelPiece AddPieceDFS(List<LevelPiece> toSpawn, LevelPiece toAddTo)
    {
        List<LevelPiece> piecesLeftToTry = toSpawn.ToList();
        piecesLeftToTry.Shuffle();
        
        List<ConnectionPoint> myPointsToTry = toAddTo.connectionPoints;
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
                        j.Attach(k);                     
                        return spawnedPiece;
                    }
                }
            }

            Destroy(spawnedPieceObject);
        }
        return null;
    }

    bool AddPiece(List<LevelPiece> toSpawn, LevelPiece toAddTo)
    {
        List<LevelPiece> piecesLeftToTry = toSpawn.ToList();
        piecesLeftToTry.Shuffle();

        List<ConnectionPoint> myPointsToTry = toAddTo.connectionPoints;
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

    bool AddPieces(List<LevelPiece> toSpawn, LevelPiece toAddTo, int maxToAdd)
    {
        List<LevelPiece> piecesLeftToTry = toSpawn.ToList();
        piecesLeftToTry.Shuffle();

        List<ConnectionPoint> myPointsToTry = toAddTo.connectionPoints;
        myPointsToTry.Shuffle();

        int numberAdded = 0;
        foreach (var i in myPointsToTry)
        {
            foreach (var j in piecesLeftToTry)
            {
                GameObject spawnedPieceObject = Instantiate(i.gameObject);
                LevelPiece spawnedPiece = spawnedPieceObject.GetComponent<LevelPiece>();
                spawnedPiece.Setup();

                List<ConnectionPoint> theirPointsToTry = spawnedPiece.connectionPoints;
                theirPointsToTry.Shuffle();

                bool fits = false;
                foreach (var k in theirPointsToTry)
                {
                    if (useWidthMatching && Mathf.Abs(i.width - k.width) > maxWidthDifference)
                    {
                        continue;
                    }

                    if (useConnectionTypes && (!i.connectableTypes.Contains(k.type) || !k.connectableTypes.Contains(i.type)))
                    {
                        continue;
                    }

                    spawnedPieceObject.transform.position = i.transform.position + (k.transform.localPosition * (-1f));

                    float angleToRotate = Vector3.SignedAngle(i.direction, k.direction * (-1f), Vector3.up);
                    spawnedPiece.transform.RotateAround(i.transform.position, Vector3.up, -angleToRotate);


                    if (spawnedPiece.FitsInPosition())
                    {
                        allSpawnedPieces.Add(spawnedPiece);
                        piecesSpawnedOrder.Add(spawnedPiece);
                        piecesToAddTo.Add(spawnedPiece);
                        i.Attach(k);
                        fits = true;
                        break;
                    }
                }
                if (fits)
                {
                    numberAdded++;
                    if (numberAdded == maxToAdd)
                    {
                        return true;
                    }
                }
                else
                {
                    Destroy(spawnedPieceObject);
                }
            }            
        }
        return false;
    }

    void DestroyAllLevelPieces()
    {
        LevelPiece[] toDestroy = FindObjectsOfType<LevelPiece>();
        foreach (var i in toDestroy)
        {
            Destroy(i.gameObject);
        }
    }
}
