using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerationManager : Singleton<LevelGenerationManager>
{
    // TODO - end piece on all generation types
    // TODO - special pieces on all generation types

    [Header("Settings")]
    public LayerMask layerMaskLevelCollision;
    public GenerationType type = GenerationType.DFS;
    public float combinationSkew = 0.5f;
    public bool spawnAtConnected = false;
    public bool spawnAtUnconnected = false;
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
    // TODO make these into dropsets
    public List<LevelPiece> piecesGeneral = new List<LevelPiece>();
    public List<LevelPiece> piecesSpecial = new List<LevelPiece>();
    public List<LevelPiece> piecesStart = new List<LevelPiece>();
    public List<LevelPiece> piecesEnd = new List<LevelPiece>();

    [HideInInspector]
    public LevelPiece startPiece;
    
    public void GenerateLevel()
    {        
        List<LevelPiece> placedPieces = new List<LevelPiece>();
        switch (type)
        {            
            case GenerationType.BFS:
                placedPieces = GenerateLevelBFS(null, Random.Range(countToSpawnMin, countToSpawnMax), true);
                break;
            case GenerationType.DFS:
                placedPieces = GenerateLevelDFS(null, Random.Range(countToSpawnMin, countToSpawnMax), true);
                break;
            case GenerationType.Combination:
                placedPieces = GenerateLevelCombination(null, Random.Range(countToSpawnMin, countToSpawnMax));
                break;
            default:
                break;
        }
        if (placedPieces != null && placedPieces.Count > 0)
        {            
            foreach (var i in placedPieces)
            {
                i.SpawnAtConnections();
                if (i.isStartPiece)
                {
                    startPiece = i;
                    i.SetFlow(0f, flowIncreaseAmount);
                    NavMeshSurface[] surfaces = i.GetComponents<NavMeshSurface>();
                    foreach (var j in surfaces)
                    {                        
                        j.BuildNavMesh();   // wait a frame
                    }
                    break;
                }
            }            
        }
        else
        {
            // failed            
        }
    }

    [ContextMenu("Generate Level Combination")]
    public List<LevelPiece> GenerateLevelCombination(LevelPiece toAddOnto, int amountToPlace)
    {
        // TODO does not place end piece 
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

            List<GenerationType> typePerPiece = new List<GenerationType>();
            for (int i = 0; i < amountToPlace; i++)
            {
                if (Random.Range(0f, 1f) < combinationSkew)
                {
                    typePerPiece.Add(GenerationType.BFS);
                }
                else
                {
                    typePerPiece.Add(GenerationType.DFS);
                }
            }

            List<KeyValuePair<GenerationType, int>> generationPlan = new List<KeyValuePair<GenerationType, int>>();
            int index = 0;
            bool first = true;
            GenerationType previousType = GenerationType.DFS;
            foreach (var i in typePerPiece)
            {
                if (first)
                {
                    generationPlan.Add(new KeyValuePair<GenerationType, int> (i, 1));
                    previousType = i;
                    first = false;                    
                }
                else if (previousType != i)
                {
                    generationPlan.Add(new KeyValuePair<GenerationType, int> (i, 1));
                    previousType = i;
                    index++;                    
                }
                else
                {                    
                    int newValue = generationPlan[index].Value + 1;
                    generationPlan[index] = new KeyValuePair<GenerationType, int> (i,newValue);
                }
            }

            bool failed = false;
            LevelPiece lastPlacedPiece = null;
            List<LevelPiece> cachedList = new List<LevelPiece>();
            //foreach (var i in generationPlan)
            for (int i = 0; i < generationPlan.Count; i++)
            {                
                if (i == generationPlan.Count - 1)
                {
                    if (generationPlan[i].Key == GenerationType.BFS)
                    {
                        cachedList = GenerateLevelBFS(lastPlacedPiece, generationPlan[i].Value, true);
                        if (cachedList != null)
                        {
                            lastPlacedPiece = cachedList.GetLast();
                        }
                        else
                        {
                            lastPlacedPiece = null;
                        }
                    }
                    else if (generationPlan[i].Key == GenerationType.DFS)
                    {
                        cachedList = GenerateLevelDFS(lastPlacedPiece, generationPlan[i].Value, true);
                        if (cachedList != null)
                        {
                            lastPlacedPiece = cachedList.GetLast();
                        }
                        else
                        {
                            lastPlacedPiece = null;
                        }
                    }
                }
                else
                {
                    if (generationPlan[i].Key == GenerationType.BFS)
                    {
                        cachedList = GenerateLevelBFS(lastPlacedPiece, generationPlan[i].Value, false);
                        if (cachedList != null)
                        {
                            lastPlacedPiece = cachedList.GetLast();
                        }
                        else
                        {
                            lastPlacedPiece = null;
                        }
                    }
                    else if (generationPlan[i].Key == GenerationType.DFS)
                    {
                        cachedList = GenerateLevelDFS(lastPlacedPiece, generationPlan[i].Value, false);
                        if (cachedList != null)
                        {
                            lastPlacedPiece = cachedList.GetLast();
                        }
                        else
                        {
                            lastPlacedPiece = null;
                        }
                    }
                }

                if (lastPlacedPiece == null)
                {
                    failed = true;
                    break;
                }
                else
                {
                    placedPieces.AddRange(cachedList.ToList());
                }
            }
            if (failed)
            {
                attempt++;
                continue;
            }
            else
            {
                Debug.Log("Level Generated Successfully on attempt " + attempt.ToString() + ".");
                int nameIndex = 0;
                foreach (var i in placedPieces)
                {
                    i.name += nameIndex++;
                }
                
                return placedPieces;
            }
        }

        int piecesToDeleteCount = placedPieces.Count;
        for (int i = piecesToDeleteCount - 1; i >= 0; i--)
        {
            Destroy(placedPieces[i].gameObject);
        }
        placedPieces.Clear();
        Debug.Log("Level failed to generate after " + attempt.ToString() + " attempts.");
        return null;
    }

    [ContextMenu("Generate Level Breath First")]
    public List<LevelPiece> GenerateLevelBFS(LevelPiece toAddOnto, int amountToPlace, bool shouldPlaceEndPiece)
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

            if (shouldPlaceEndPiece)
            {
                while (placedPieces.Count < amountToPlace - 1)
                {
                    List<LevelPiece> piecesToUse = new List<LevelPiece>();
                    piecesToUse = piecesGeneral;

                    List<LevelPiece> newCurrentPieces = new List<LevelPiece>();
                    foreach (var i in currentPieces)
                    {
                        int piecesRemaining = (amountToPlace - 1) - placedPieces.Count;
                        if (piecesRemaining == 0)
                        {
                            break;
                        }
                        else
                        {
                            List<LevelPiece> addedPieces = AddPieces(piecesToUse, i, piecesRemaining);
                            placedPieces.AddRange(addedPieces.ToList());
                            newCurrentPieces.AddRange(addedPieces.ToList());
                        }
                    }

                    currentPieces.Clear();
                    currentPieces = newCurrentPieces.ToList();

                    if (currentPieces.Count == 0)
                    {
                        break;
                    }
                }

                if (currentPieces.Count > 0)
                {
                    while (placedPieces.Count < amountToPlace)
                    {
                        List<LevelPiece> piecesToUse = new List<LevelPiece>();
                        piecesToUse = piecesEnd;

                        List<LevelPiece> newCurrentPieces = new List<LevelPiece>();
                        foreach (var i in currentPieces)
                        {
                            int piecesRemaining = amountToPlace - placedPieces.Count;
                            if (piecesRemaining == 0)
                            {
                                break;
                            }
                            else
                            {
                                List<LevelPiece> addedPieces = AddPieces(piecesToUse, i, piecesRemaining);
                                placedPieces.AddRange(addedPieces.ToList());
                                newCurrentPieces.AddRange(addedPieces.ToList());
                            }
                        }

                        currentPieces.Clear();
                        currentPieces = newCurrentPieces.ToList();

                        if (currentPieces.Count == 0)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                while (placedPieces.Count < amountToPlace)
                {
                    List<LevelPiece> piecesToUse = new List<LevelPiece>();
                    piecesToUse = piecesGeneral;

                    List<LevelPiece> newCurrentPieces = new List<LevelPiece>();
                    foreach (var i in currentPieces)
                    {
                        int piecesRemaining = amountToPlace - placedPieces.Count;
                        if (piecesRemaining == 0)
                        {
                            break;
                        }
                        else
                        {
                            List<LevelPiece> addedPieces = AddPieces(piecesToUse, i, piecesRemaining);
                            placedPieces.AddRange(addedPieces.ToList());
                            newCurrentPieces.AddRange(addedPieces.ToList());
                        }
                    }

                    currentPieces.Clear();
                    currentPieces = newCurrentPieces.ToList();

                    if (currentPieces.Count == 0)
                    {
                        break;
                    }
                }
            }

            if (placedPieces.Count == amountToPlace)
            {
                //Debug.Log("Level Generated Successfully on attempt " + attempt.ToString() + ".");
                return placedPieces;
            }
            else
            {
                //Debug.Log("Level failed to generate on attempt " + attempt.ToString() + ".");
                attempt++;
            }

        }

        int piecesToDeleteCount = placedPieces.Count;
        for (int i = piecesToDeleteCount - 1; i >= 0; i--)
        {
            Destroy(placedPieces[i].gameObject);
        }
        placedPieces.Clear();
        
        return null;
    }

    [ContextMenu("Generate Level Depth First")]
    public List<LevelPiece> GenerateLevelDFS(LevelPiece toAddOnto, int amountToPlace, bool shouldPlaceEndPiece)
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
             
            while (placedPieces.Count < amountToPlace)
            {
                List<LevelPiece> piecesToUse = new List<LevelPiece>();
                if (shouldPlaceEndPiece && placedPieces.Count + 1 == amountToPlace)
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
                    if (placedPiecesOrder.Count == 0)
                    {
                        break;
                    }

                    placedPiecesOrder.RemoveAt(placedPiecesOrder.Count - 1);  // todo check has open connections

                    if (placedPiecesOrder.Count == 0)
                    {
                        break;
                    }
                }
            }

            if (placedPieces.Count == amountToPlace)
            {
                //Debug.Log("Level Generated Successfully on attempt " + attempt.ToString() + ".");                                
                return placedPieces;
            }
            else
            {
                //Debug.Log("Level failed to generate on attempt " + attempt.ToString() + ".");
                attempt++;
            }

        }

        int piecescToDeleteCount = placedPieces.Count;
        for (int i = piecescToDeleteCount - 1; i >= 0; i--)
        {
            Destroy(placedPieces[i].gameObject);
        }
        placedPieces.Clear();
        placedPiecesOrder.Clear();
        
        return null;
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
