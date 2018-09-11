using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    [Header("References")]
    public List<Transform> spawnPoints = new List<Transform>();
    public List<ControllerMultiPlayer> allControllers = new List<ControllerMultiPlayer>();

    [Header("Prefabs")]
    public GameObject controllerMultiPlayer;

    public void SpawnPlayer(PlayerAttibutes newAttributes)
    {
        if (controllerMultiPlayer == null)
        {
            Debug.LogError("No prefab for the player controller when attempting to spawn: " + newAttributes.indexPlayer.ToString());
            return;
        }
        GameObject spawnedControllerObject = Instantiate(controllerMultiPlayer);
        spawnedControllerObject.name = "[P" + newAttributes.indexPlayer.ToString() + "]PlayerController";

        if (allControllers.Count == 0)
        {
            spawnedControllerObject.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        }
        else
        {
            spawnedControllerObject.transform.position = allControllers[Random.Range(0, allControllers.Count)].transform.position;
        }
        
        // Invulnerable state

        ControllerMultiPlayer spawnedController = spawnedControllerObject.GetComponent<ControllerMultiPlayer>();
        spawnedController.Setup(newAttributes);
    }



}

