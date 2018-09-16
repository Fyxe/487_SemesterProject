using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopManager : LevelManager
{
    [Header("Shop settings")]
    public float chanceOfSpawningLocaion = 0.5f;
    public DropSet locations;
    public List<Transform> locationsPositions = new List<Transform>();

    protected override void OnFocused()
    {
        base.OnFocused();
        foreach (var i in locationsPositions)
        {
            if (Random.Range(0f,1f) < chanceOfSpawningLocaion)
            {
                GameObject spawnedLocationObject = Instantiate(locations.GetDrop());
                spawnedLocationObject.transform.position = i.position;
                spawnedLocationObject.transform.rotation = i.rotation;
            }
        }
    }

    public override void EndLevel(bool isVictory)
    {
        isPlaying = false;
        if (isVictory)
        {
            UpdatePlayerInformation();            
            GameManager.instance.GoToNextLevel();
        }
        else
        {
            PlayerManager.instance.ResetAllPlayers();
            GameManager.instance.GoToMainMenu();
        }
    }

    public override void SpawnPlayer(PlayerAttributes newAttributes)
    {
        base.SpawnPlayer(newAttributes);        
        PlayerManager.instance.GetAttributeOfPlayer(newAttributes.indexPlayer).isDead = false;
    }

    public override void SpawnPlayersInitially()
    {
        List<Transform> spawnCopies = spawnPoints.ToList();
        foreach (var i in PlayerManager.instance.allPlayerAttributes)
        {
            if (i.isSpawned)
            {
                if (i.prefabController == null)
                {
                    Debug.LogError("No prefab for the player controller when attempting to spawn: P" + i.indexJoystick.ToString());
                    return;
                }
                i.isDead = false;

                GameObject spawnedControllerObject = Instantiate(i.prefabController);                

                if (spawnCopies.Count == 0)
                {
                    if (spawnPoints.Count == 0)
                    {
                        spawnedControllerObject.transform.position = Vector3.zero;
                    }
                    else
                    {
                        spawnedControllerObject.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
                    }
                }
                else
                {
                    int index = Random.Range(0, spawnCopies.Count);

                    spawnedControllerObject.transform.position = spawnCopies[index].position;
                    spawnCopies.RemoveAt(index);
                }

                ControllerMultiPlayer spawnedController = spawnedControllerObject.GetComponent<ControllerMultiPlayer>();
                spawnedController.Setup(i, playerUIBoxes[i.indexPlayer]);
                spawnedController.SetInvulnerable(PlayerManager.instance.timeInvulnerable);
                allControllers.Add(spawnedController);
                FindObjectOfType<NavMeshCameraController>().toFollow.Add(spawnedController.transform);
            }
        }
    }
}
