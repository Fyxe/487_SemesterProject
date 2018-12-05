using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UI;

public class GameLevelManager : LevelManager
{
    [Header("Settings")]
    public bool allowPlayerSpawns = false;
    public float timeStaleFull = 120f;
    public float currentStaleness = 0f;
    public int distanceFromPlayersToSpawn = 2;
    public bool spawnInPiecesPlayersAreIn = true;
    public float endWaitDelay = 2f;

    [Header("Spawn Settings")]
    public int setEnemies = 20;
    public float delaySpawn = 1f;
    float nextSpawn = 0f;

    [Header("References")]
    public ScreenBase screenEnd;

    LevelGenerationManager generator;
    public Image staleFillImage;

    public Dictionary<AI, List<AI>> allAI = new Dictionary<AI, List<AI>>();

    public List<LevelPiece> piecesToSpawnIn = new List<LevelPiece>();    
    public List<LevelPiece> piecesPlayersAreIn = new List<LevelPiece>();
    
    void Awake()
    {
        generator = FindObjectOfType<LevelGenerationManager>();
        staleFillImage.fillAmount = 0f;
    }

    void Update()
    {
        if (isPlaying)
        {
            currentStaleness += Time.deltaTime;
            if (staleFillImage != null)
            {
                staleFillImage.fillAmount = Mathf.Lerp(staleFillImage.fillAmount, (float)currentStaleness / (float)timeStaleFull, 0.2f);
            }
            if (currentStaleness >= timeStaleFull)
            {
                EndLevel(false);
            }
            
            // TODO check if algorithm makes sense?
            AI prefabAIToSpawn = DropManager.instance.GetDrop(Thing.ai).GetComponent<AI>();
            if ((!allAI.ContainsKey(prefabAIToSpawn) || (allAI.ContainsKey(prefabAIToSpawn) && allAI[prefabAIToSpawn].Count < setEnemies)) && Time.time > nextSpawn)
            {
                nextSpawn = Time.time + (delaySpawn * GameplayManager.instance.enemySpawnRate * PlayerManager.instance.playersInGame);
                SpawnEnemy(prefabAIToSpawn, PositionToSpawn.ALL);
            }
        }
    }

    public override void StartLevel()
    {
        base.StartLevel();
    }

    public override void EndLevel(bool isVictory)
    {
        isPlaying = false;
        if (isVictory)
        {
            foreach (var i in allControllers)
            {
                i.pointsCurrent += PointsManager.instance.pointsOnLevelEnd;
            }
            ProgressionManager.instance.scoreCurrentInLevel += ProgressionManager.instance.scoreOnLevelCompletion;
            GameplayManager.instance.GoToNextLevel();
        }
        else
        {
            StartCoroutine(EndWait());
        }
    }

    IEnumerator EndWait()
    {
        foreach (var i in allControllers)
        {
            i.HurtToDeath();
        }

        yield return new WaitForSeconds(endWaitDelay);
        ScreenManager.instance.ScreenSet(screenEnd, false, false);

    }

    public void CallbackEndGame()
    {
        GameplayManager.instance.EndGame();
    }

    protected override void OnFocused()
    {        
        if (generator != null)
        {
            if (GameManager.instance.gameStartedCorrectly)
            {
                generator.countToSpawnMax = GameplayManager.instance.piecesMaxCurrent;
                generator.countToSpawnMin = GameplayManager.instance.piecesMinCurrent;
            }
            if (generator.GenerateLevel())
            {
                StartCoroutine(WaitAfterFocused());
            }
            else
            {
                LoadSceneManager.instance.LoadScene(SceneManager.GetActiveScene().name);
            }
        }           
    }

    IEnumerator WaitAfterFocused()
    {
        while (generator.startPiece == null)
        {
            yield return null;
        }
        spawnPoints = generator.startPiece.spawnPositionsPlayer.ToList();
        base.OnFocused();
    }

    public override void SpawnPlayer(PlayerAttributes newAttributes)
    {
        if (allowPlayerSpawns)
        {
            base.SpawnPlayer(newAttributes);
        }
        else
        {
            playerUIBoxes[newAttributes.indexPlayer].Set(PlayerUIBox.BoxSetting.dead);
            newAttributes.isDead = true;
        }
    }

    public override void SpawnPlayersInitially()
    {
        List<SpawnPosition> spawnPositions = LevelGenerationManager.instance.startPiece.spawnPositionsPlayer.ToList();
        foreach (var i in PlayerManager.instance.allPlayerAttributes)
        {
            if (i.isSpawned)
            {
                if (i.prefabController == null)
                {
                    Debug.LogError("No prefab for the player controller when attempting to spawn: P" + i.indexJoystick.ToString());
                    return;
                }
                GameObject spawnedControllerObject = Instantiate(i.prefabController);
                spawnedControllerObject.name = "[P" + i.indexJoystick.ToString() + "]PlayerController";
                

                spawnPositions[0].SpawnObject(spawnedControllerObject.transform);
                spawnedControllerObject.transform.position += Vector3.up * dropHeight;
                spawnedControllerObject.transform.rotation = Random.rotation;                
                spawnPositions.RemoveAt(0);

                

                ControllerMultiPlayer spawnedController = spawnedControllerObject.GetComponent<ControllerMultiPlayer>();
                spawnedController.Setup(i, playerUIBoxes[i.indexPlayer]);
                spawnedController.SetInvulnerable(PlayerManager.instance.timeInvulnerable);
                spawnedController.SetFalling();
                allControllers.Add(spawnedController);
                FindObjectOfType<NavMeshCameraController>().toFollow.Add(spawnedController.transform);
            }
        }
        StartCoroutine(WaitForPlayersToDrop());
    }

    public List<LevelPiece> GetPiecesDistance(int distanceFromPlayers)
    {
        List<LevelPiece> retList = new List<LevelPiece>();
        retList.AddRange(piecesPlayersAreIn);

        for (int i = 0; i < distanceFromPlayers; i++)
        {
            List<LevelPiece> tempList = retList.ToList();
            foreach (var j in retList)
            {
                foreach (var k in j.connectedTo)
                {
                    if (tempList.Contains(k) || retList.Contains(k))
                    {
                        Debug.Log("a");
                        continue;
                    }
                    if (k.spawnColliders.Count > 0 && (spawnInPiecesPlayersAreIn || (!spawnInPiecesPlayersAreIn && !k.hasPlayers)))
                    {
                        tempList.Add(k);
                    }
                }
            }
            retList.AddRange(tempList.ToList());     // ?   
        }

        return retList.ToList();
    }

    public bool SpawnEnemy(AI prefabToSpawn, PositionToSpawn whereToSpawn)
    {
        if (piecesToSpawnIn.Count == 0)
        {
            //Debug.LogError("No pieces to spawn enemy in.");
            return false;
        }
        int index = Random.Range(0,piecesToSpawnIn.Count);
        LevelPiece toSpawnIn = piecesToSpawnIn[index];
        Vector3? spawnPosition = toSpawnIn.GetRandomSpawnPosition();
        if (spawnPosition == null)
        {
            //Debug.LogError("No spawn position found in level piece.");
            return false;
        }
        NavMeshHit h;
        if (NavMesh.SamplePosition(spawnPosition.Value, out h, 20f, prefabToSpawn.agent.areaMask))
        {
            GameObject spawnedAIObject = ObjectPoolingManager.instance.CreateObject(prefabToSpawn as PooledObject).gameObject;
            spawnedAIObject.transform.position = h.position;
            AI spawnedAI = spawnedAIObject.GetComponent<AI>();
            if (allAI.ContainsKey(prefabToSpawn))
            {
                allAI[prefabToSpawn].Add(spawnedAI);
            }
            else
            {
                allAI.Add(prefabToSpawn,new List<AI> { spawnedAI });
            }
            spawnedAI.hpMax += GameplayManager.instance.enemyHealthModifier;
            spawnedAI.hpCurrent = spawnedAI.hpMax;
            return true;
        }
        else
        {
            Debug.LogError("No navmesh sample hit.");
            return false;
        }
    }

    public void AddToPiecesPlayersAreIn(LevelPiece toAdd)
    {
        
        piecesPlayersAreIn.Add(toAdd);
        piecesToSpawnIn.Clear();
        //piecesToSpawnIn = GetPiecesDistance(distanceFromPlayersToSpawn);
        List<LevelPiece> pieces = new List<LevelPiece>();
        foreach (var i in piecesPlayersAreIn)
        {
            pieces.AddRange(GetConnectedPieces(pieces, distanceFromPlayersToSpawn, i).ToList());
        }
        piecesToSpawnIn = pieces.Distinct().ToList();   // TODO optimise
        
    }

    public void RemoveFromPiecesPlayersAreIn(LevelPiece toRemove)
    {
        piecesPlayersAreIn.Remove(toRemove);
        piecesToSpawnIn.Clear();
        //piecesToSpawnIn = GetPiecesDistance(distanceFromPlayersToSpawn);
        List<LevelPiece> pieces = new List<LevelPiece>();
        foreach (var i in piecesPlayersAreIn)
        {
            pieces.AddRange(GetConnectedPieces(pieces, distanceFromPlayersToSpawn, i));
        }
        piecesToSpawnIn = pieces.Distinct().ToList();   // TODO optimise
    }

    public List<LevelPiece> GetConnectedPieces(List<LevelPiece> retPieces, int iterationsLeft, LevelPiece toAddFrom)
    {
        if (iterationsLeft == 0)
        {
            return retPieces;
        }
        else
        {
            if (!retPieces.Contains(toAddFrom))
            {
                retPieces.Add(toAddFrom);
            }
            iterationsLeft--;
            foreach (var i in toAddFrom.connectedTo)
            {
                retPieces = GetConnectedPieces(retPieces, iterationsLeft, i);               
            }
            return retPieces;
        }
    }
}
