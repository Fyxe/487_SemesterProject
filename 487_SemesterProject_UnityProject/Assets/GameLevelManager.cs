using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLevelManager : LevelManager
{

    [Header("Settings")]
    public bool allowPlayerSpawns = false;
    public float timeStaleFull = 120f;
    public float currentStaleness = 0f;

    [Header("References")]
    LevelGenerationManager generator;
    public Image staleFillImage;


    void Awake()
    {
        generator = FindObjectOfType<LevelGenerationManager>();
        staleFillImage.fillAmount = 0f;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            FindObjectOfType<ControllerMultiPlayer>().Hurt(1);
        }
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
        }
    }

    protected override void OnFocused()
    {        
        if (generator != null)
        {
            generator.GenerateLevel();
        }
        base.OnFocused();
        StartLevel();
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



}
