using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    [Header("Settings")]
    public float timeInvulnerable = 1f;

    [Header("References")]
    public List<ControllerMultiPlayer> allControllers = new List<ControllerMultiPlayer>();
    public List<Transform> spawnPoints = new List<Transform>();

    //[Header("Prefabs")]
    

    [ContextMenu("StartLevel")]
    public void StartLevel()
    {
        SpawnPlayersInitially();
    }

    public void SpawnPlayer(PlayerAttibutes newAttributes)
    {
        if (newAttributes.prefabController == null)
        {
            Debug.LogError("No prefab for the player controller when attempting to spawn: P" + newAttributes.indexJoystick.ToString());
            return;
        }
        GameObject spawnedControllerObject = Instantiate(newAttributes.prefabController);
        spawnedControllerObject.name = "[P" + newAttributes.indexJoystick.ToString() + "]PlayerController";

        if (allControllers.Count == 0)
        {
            spawnedControllerObject.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        }
        else
        {
            spawnedControllerObject.transform.position = allControllers[Random.Range(0, allControllers.Count)].transform.position;
        }                

        ControllerMultiPlayer spawnedController = spawnedControllerObject.GetComponent<ControllerMultiPlayer>();
        spawnedController.Setup(newAttributes);
        spawnedController.SetInvulnerable(timeInvulnerable);
        allControllers.Add(spawnedController);
    }

    public void SpawnPlayersInitially()
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
                GameObject spawnedControllerObject = Instantiate(i.prefabController);
                spawnedControllerObject.name = "[P" + i.indexJoystick.ToString() + "]PlayerController";

                int index = Random.Range(0, spawnCopies.Count);

                spawnedControllerObject.transform.position = spawnCopies[index].position;
                spawnCopies.RemoveAt(index);                

                ControllerMultiPlayer spawnedController = spawnedControllerObject.GetComponent<ControllerMultiPlayer>();
                spawnedController.Setup(i);
                spawnedController.SetInvulnerable(timeInvulnerable);
                allControllers.Add(spawnedController);
            }
        }
    }


}

