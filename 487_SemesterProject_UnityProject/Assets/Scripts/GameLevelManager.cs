using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UI;

public class GameLevelManager : LevelManager
{
    [Header("Settings")]
    public bool allowPlayerSpawns = false;
    public float timeStaleFull = 120f;
    public float currentStaleness = 0f;
    public int distanceFromPlayersToSpawn = 2;
    public bool spawnInPiecesPlayersAreIn = true;

    [Header("Spawn Settings")]
    public int setEnemies = 20;
    public float delaySpawn = 1f;
    float nextSpawn = 0f;

    [Header("References")]
    public ScreenBase screenEnd;
    public AI prefabEnemyBasic;

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

            if ((!allAI.ContainsKey(prefabEnemyBasic) || (allAI.ContainsKey(prefabEnemyBasic) && allAI[prefabEnemyBasic].Count < setEnemies)) && Time.time > nextSpawn)
            {
                nextSpawn = Time.time + delaySpawn;
                SpawnEnemy(prefabEnemyBasic, PositionToSpawn.ALL);
            }
        }
    }

    public override void EndLevel(bool isVictory)
    {
        isPlaying = false;
        if (isVictory)
        {
            //UpdatePlayerInformation();
            ProgressionManager.instance.scoreCurrentInLevel += ProgressionManager.instance.scoreOnLevelCompletion;
            GameplayManager.instance.GoToNextLevel();
        }
        else
        {
            ScreenManager.instance.ScreenSet(screenEnd,false,false);
            // TODO kill all players
            foreach (var i in allControllers)
            {
                i.HurtToDeath();
            }
        }
    }

    public void CallbackEndGame()
    {
        GameplayManager.instance.EndGame();
    }

    protected override void OnFocused()
    {        
        if (generator != null)
        {
            generator.countToSpawnMax = GameplayManager.instance.piecesMaxCurrent;
            generator.countToSpawnMin = GameplayManager.instance.piecesMinCurrent;
            generator.GenerateLevel();
        }
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
                spawnPositions.RemoveAt(0);

                

                ControllerMultiPlayer spawnedController = spawnedControllerObject.GetComponent<ControllerMultiPlayer>();
                spawnedController.Setup(i, playerUIBoxes[i.indexPlayer]);
                spawnedController.SetInvulnerable(PlayerManager.instance.timeInvulnerable);
                allControllers.Add(spawnedController);
                FindObjectOfType<NavMeshCameraController>().toFollow.Add(spawnedController.transform);
            }
        }
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
                    if (!tempList.Contains(k) && k.spawnColliders.Count > 0 && (spawnInPiecesPlayersAreIn || (!spawnInPiecesPlayersAreIn && !k.hasPlayers)))
                    {
                        tempList.Add(k);
                    }
                }
            }
            retList.AddRange(tempList);            
        }
        return retList;
    }

    public bool SpawnEnemy(AI prefabToSpawn, PositionToSpawn whereToSpawn)
    {
        if (piecesToSpawnIn.Count == 0)
        {
            Debug.LogError("No pieces to spawn enemy in.");
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
        piecesToSpawnIn = GetPiecesDistance(distanceFromPlayersToSpawn);
    }

    public void RemoveFromPiecesPlayersAreIn(LevelPiece toRemove)
    {
        piecesPlayersAreIn.Remove(toRemove);
        piecesToSpawnIn = GetPiecesDistance(distanceFromPlayersToSpawn);
    }
}
