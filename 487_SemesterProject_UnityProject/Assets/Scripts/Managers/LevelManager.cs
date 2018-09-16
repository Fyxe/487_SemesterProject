using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{

    //[Header("Settings")]
    public bool isPlaying = false;

    [Header("References")]
    public DropSet baseDropSetEnemy;
    public DropSet baseDropSetBarrel;
    public List<ControllerMultiPlayer> allControllers = new List<ControllerMultiPlayer>();
    public List<Transform> spawnPoints = new List<Transform>();
    public List<PlayerUIBox> playerUIBoxes = new List<PlayerUIBox> ();

    //[Header("Prefabs")]

    void Start()
    {        
        foreach (var i in playerUIBoxes)
        {
            i.Set(PlayerUIBox.BoxSetting.empty);
        }

        StartCoroutine(WaitUntilFocused());        
    }

    IEnumerator WaitUntilFocused()
    {
        while (SceneManager.GetActiveScene().name == "LoadingScene")
        {
            yield return null;
        }
        OnFocused();        
    }

    protected virtual void OnFocused()
    {
        SpawnPlayersInitially();
    }

    public void StartLevel()
    {
        isPlaying = true;
    }

    public virtual void EndLevel(bool isVictory)
    {
        isPlaying = false;
        if (isVictory)
        {
            UpdatePlayerInformation();
            ProgressionManager.instance.currentScore += ProgressionManager.instance.scoreOnLevelCompletion;
            GameManager.instance.GoToNextLevel();
        }
        else
        {
            PlayerManager.instance.ResetAllPlayers();
            GameManager.instance.GoToMainMenu();
        }
    }

    protected void UpdatePlayerInformation()
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

    public virtual void SpawnPlayer(PlayerAttributes newAttributes)
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
        spawnedController.SetInvulnerable(PlayerManager.instance.timeInvulnerable);
        allControllers.Add(spawnedController);
        FindObjectOfType<NavMeshCameraController>().toFollow.Add(spawnedController.transform);
    }

    public virtual void SpawnPlayersInitially()
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
                spawnedController.SetInvulnerable(PlayerManager.instance.timeInvulnerable);
                allControllers.Add(spawnedController);
                FindObjectOfType<NavMeshCameraController>().toFollow.Add(spawnedController.transform);
            }
        }
    }

    public void CheckIfAllPlayersAreDead()
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

    public Transform GetTarget()
    {
        foreach (var i in allControllers)
        {
            if (!i.isDead)
            {
                return i.transform;
            }
        }
        return null;
    }
}

