using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UI;

public class LevelManager : Singleton<LevelManager>
{

    [Header("LevelManager Settings")]
    public float dropTime = 5f;
    public float dropHeight = 10f;
    public float dropLerpTime = 2f;
    bool isPaused = false;
    bool m_isPlaying = false;
    public bool isPlaying
    {
        get
        {
            if (isPaused)
            {
                return false;
            }
            else
            {
                return m_isPlaying;
            }
        }
        set
        {
            m_isPlaying = value;
        }
    }
    

    [Header("References")]
    public DropSet baseDropSetEnemy;
    public DropSet baseDropSetBarrel;
    public List<ControllerMultiPlayer> allControllers = new List<ControllerMultiPlayer>();
    public List<SpawnPosition> spawnPoints = new List<SpawnPosition>();
    public List<PlayerUIBox> playerUIBoxes = new List<PlayerUIBox> ();
    public ScreenPause screenPause;

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
            //UpdatePlayerInformation();
            ProgressionManager.instance.scoreCurrentInLevel += ProgressionManager.instance.scoreOnLevelCompletion;
            GameplayManager.instance.GoToNextLevel();
        }
        else
        {
            GameplayManager.instance.EndGame();
            // TODO kill all players
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

        if (allControllers.Count == 0)
        {
            if (spawnPoints.Count == 0)
            {
                spawnedControllerObject.transform.position = Vector3.zero;
            }
            else
            {
                spawnedControllerObject.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;
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
        List<SpawnPosition> spawnCopies = spawnPoints.ToList();
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
                        spawnedControllerObject.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position + Vector3.up * dropHeight;
                        spawnedControllerObject.transform.rotation = Random.rotation;
                    }
                }
                else
                {
                    int index = Random.Range(0, spawnCopies.Count);

                    spawnedControllerObject.transform.position = spawnCopies[index].transform.position;
                    spawnCopies.RemoveAt(index);                
                }
                
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

    public IEnumerator WaitForPlayersToDrop()  
    {
        yield return new WaitForSeconds(dropTime);
        float currentTime = 0f;
        List<Vector3> startPositions = new List<Vector3>();
        List<Quaternion> startRotations = new List<Quaternion>();
        foreach (var i in allControllers)
        {
            i.rb.isKinematic = true;
            startPositions.Add(i.transform.position);
            startRotations.Add(i.transform.rotation);
        }
        while (currentTime < dropLerpTime)
        {
            currentTime += Time.deltaTime;
            for (int i = 0; i < allControllers.Count; i++)
            {
                allControllers[i].transform.position = Vector3.Lerp(startPositions[i], spawnPoints[i].transform.position, currentTime / dropLerpTime);
                allControllers[i].transform.rotation = Quaternion.Lerp(startRotations[i], spawnPoints[i].transform.rotation, currentTime / dropLerpTime);
            }
            yield return null;
        }
        foreach (var i in allControllers)
        {
            i.SetReady();
        }
        StartLevel();
    }

    public void CheckIfAllPlayersAreDead()
    {
        if (!isPlaying)
        {
            return;
        }
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

    public virtual void Pause()
    {
        //GameManager.instance.Pause();
        isPaused = true;
        ScreenManager.instance.ScreenAdd(screenPause,false);
    }

    public virtual void Resume()
    {
        //GameManager.instance.Resume();
        isPaused = false;
        ScreenManager.instance.ScreenRemove(screenPause, false);
    }
}

