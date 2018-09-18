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
                GenerateLevelBFS(null, Random.Range(countToSpawnMin, countToSpawnMax));
                break;
            case GenerationType.DFS:
                GenerateLevelDFS(null, Random.Range(countToSpawnMin, countToSpawnMax));
                break;
            case GenerationType.Synthesis:
                GenerateLevelCombination(null, Random.Range(countToSpawnMin, countToSpawnMax));
                break;
            default:
                break;
        }
    }

    [ContextMenu("Generate Level Combination")]
    public bool GenerateLevelCombination(LevelPiece toAddOnto, int amountToPlace)
    {
        int attempt = 0;
        while (attempt < maxFails)
        {
            
            return true;
        }
        return false;
    }

    [ContextMenu("Generate Level Breath First")]
    public bool GenerateLevelBFS(LevelPiece toAddOnto, int amountToPlace)
    {
        List<LevelPiece> placedPieces = new List<LevelPiece>();        

        int attempt = 0;
        while (attempt < maxFails)
        {
            int placedPiecesCount = placedPieces.Count;
            for (int i = placedPiecesCount - 1; i >= 0; i--)
            {
                Destroy(placedPieces[i].gameObject);
            }
            placedPieces.Clear();            

            List<LevelPiece> currentPieces = new List<LevelPiece> ();
            if (toAddOnto == null)
            {
                GameObject spawnedStartPieceObject = Instantiate(piecesStart.GetRandomValue().gameObject);
                spawnedStartPieceObject.name += " StartPiece";
                spawnedStartPieceObject.transform.position = startPosition.position;

                LevelPiece spawnedStartPiece = spawnedStartPieceObject.GetComponent<LevelPiece>();
                spawnedStartPiece.Setup();

                placedPieces.Add(spawnedStartPiece);                

                currentPieces.Add(spawnedStartPiece);
            }
            else
            {
                currentPieces.Add(toAddOnto);
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

                List<LevelPiece> newCurrentPieces = new List<LevelPiece>();
                foreach (var i in currentPieces)
                {
                    int piecesRemaining = amountToPlace - placedPieces.Count;
                    List<LevelPiece> addedPieces = AddPieces(piecesToUse, i, piecesRemaining);
                    placedPieces.AddRange(addedPieces.ToList());                    
                    newCurrentPieces.AddRange(addedPieces.ToList());
                }
                currentPieces.Clear();
                currentPieces = newCurrentPieces.ToList();

                if (currentPieces.Count == 0)                
                {
                    break;
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

                LevelPiece addedPiece = AddPiece(piecesToUse, currentPiece);
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

    LevelPiece AddPiece(List<LevelPiece> toSpawn, LevelPiece toAddTo)
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
                    if (k.attachedTo != null)
                    {
                        continue;
                    }
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
                    else
                    {
                        
                    }
                }                
            }
            Destroy(spawnedPieceObject);
        }
        return null;
    }

    List<LevelPiece> AddPieces(List<LevelPiece> toSpawn, LevelPiece toAddTo, int maxToAdd)
    {
        if (maxToAdd == 0)
        {
            return new List<LevelPiece>();
        }
        List<LevelPiece> retList = new List<LevelPiece>();

        List<LevelPiece> piecesLeftToTry = toSpawn.ToList();
        piecesLeftToTry.Shuffle();

        List<ConnectionPoint> myPointsToTry = toAddTo.connectionPoints;
        myPointsToTry.Shuffle();

        foreach (var i in myPointsToTry)
        {
            if (i.attachedTo != null)
            {
                continue;
            }
            foreach (var j in piecesLeftToTry)
            {
                GameObject spawnedPieceObject = Instantiate(j.gameObject);
                LevelPiece spawnedPiece = spawnedPieceObject.GetComponent<LevelPiece>();
                spawnedPiece.Setup();

                List<ConnectionPoint> theirPointsToTry = spawnedPiece.connectionPoints;
                theirPointsToTry.Shuffle();

                bool fit = false;
                foreach (var k in theirPointsToTry)
                {
                    if (k.attachedTo != null)
                    {
                        continue;
                    }
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

                    if (spawnedPiece.FitsInPosition() && i.Attach(k))
                    {
                        retList.Add(spawnedPiece);
                        fit = true;
                        if (retList.Count == maxToAdd)
                        {
                            return retList;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (!fit)
                {
                    Destroy(spawnedPieceObject);
                }
            }
        }
        return retList;
    }
}
