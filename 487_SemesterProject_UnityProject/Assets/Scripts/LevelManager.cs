using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{

    [Header("Settings")]
    public float timeInvulnerable = 1f;
    
    [Header("References")]
    public List<ControllerMultiPlayer> allControllers = new List<ControllerMultiPlayer>();
    public List<Transform> spawnPoints = new List<Transform>();
    public List<PlayerUIBox> playerUIBoxes = new List<PlayerUIBox> ();
    AgentCameraController cameraController;

    //[Header("Prefabs")]
    
    void Start()
    {
        foreach(var i in playerUIBoxes)
        {
            i.Set(PlayerUIBox.BoxSetting.empty);
        }        
        StartLevel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            FindObjectOfType<ControllerMultiPlayer>().Hurt(1);
        }
    }

    protected virtual IEnumerator WaitUntilFocused()
    {
        while (SceneManager.GetActiveScene().name == "LoadingScene")
        {
            yield return null;
        }
        SpawnPlayersInitially();
    }

    public void StartLevel()
    {
        StartCoroutine(WaitUntilFocused());
    }

    public void EndLevel(bool isVictory)
    {
        if (isVictory)
        {
            UpdatePlayerInformation();
            GameManager.instance.currentScore += 1000;
            GameManager.instance.GoToNextLevel();
        }
        else
        {
            PlayerManager.instance.ResetAllPlayers();
            GameManager.instance.GoToMainMenu();
        }
    }

    void UpdatePlayerInformation()
    {
        foreach (var i in allControllers)
        {
            PlayerAttributes at = PlayerManager.instance.GetAttributeOfPlayer(i.indexPlayer);
            if (at == null)
            {                
                continue;
            }
            at.pointsCurrent = i.pointsCurrent;
            at.hpCurrent = i.hpCurrent;
            at.hpMax = i.hpMax;
            at.speedMoveCurrent = i.speedMoveCurrent;
            at.damageBaseCurrent = i.damageBaseCurrent;
            at.countReviveCurrent = i.countReviveCurrent;
        }
    }

    public void SpawnPlayer(PlayerAttributes newAttributes)
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
            spawnedControllerObject.transform.position = allControllers[Random.Range(0, allControllers.Count)].transform.position;
        }                

        ControllerMultiPlayer spawnedController = spawnedControllerObject.GetComponent<ControllerMultiPlayer>();
        spawnedController.Setup(newAttributes, playerUIBoxes[newAttributes.indexPlayer]);
        spawnedController.SetInvulnerable(timeInvulnerable);
        allControllers.Add(spawnedController);
        FindObjectOfType<AgentCameraController>().toFollow.Add(spawnedController.transform);
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
                spawnedController.SetInvulnerable(timeInvulnerable);
                allControllers.Add(spawnedController);
                FindObjectOfType<AgentCameraController>().toFollow.Add(spawnedController.transform);
            }
        }
    }

    public void CheckForEnd()
    {
        foreach (var i in allControllers)
        {
            if (!i.isDead)
            {
                return;
            }
        }
        EndLevel(false);
    }
}

