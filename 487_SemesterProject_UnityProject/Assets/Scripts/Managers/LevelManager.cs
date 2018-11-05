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
    public bool dropPlayers = true;
    public float dropTime = 4f;
    public float dropHeight = 10f;
    public float dropLerpTime = .85f;
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

    public AudioClip backgroundMusic;
    //[Header("Prefabs")]

    void Start()
    {        
        foreach (var i in playerUIBoxes)
        {
            i.Set(PlayerUIBox.BoxSetting.empty);
        }

        StartCoroutine(WaitUntilFocused());
        AudioManager.instance.PlayClipLocalLooped(backgroundMusic);
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

    public virtual void StartLevel()
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

        Transform endTransform;

        if (allControllers.Count == 0)
        {
            if (spawnPoints.Count == 0)
            {
                spawnedControllerObject.transform.position = Vector3.zero + Vector3.up * dropHeight;
                endTransform = new GameObject().transform;
            }
            else
            {
                int randomIndex = Random.Range(0, spawnPoints.Count);
                spawnedControllerObject.transform.position = spawnPoints[randomIndex].transform.position + Vector3.up * dropHeight;
                endTransform = spawnPoints[randomIndex].transform;
            }            
        }
        else
        {
            int randomIndex = Random.Range(0, allControllers.Count);
            spawnedControllerObject.transform.position = allControllers[randomIndex].transform.position + Vector3.up * dropHeight;
            endTransform = allControllers[randomIndex].transform;
        }                

        ControllerMultiPlayer spawnedController = spawnedControllerObject.GetComponent<ControllerMultiPlayer>();
        spawnedController.Setup(newAttributes, playerUIBoxes[newAttributes.indexPlayer]);
        spawnedController.SetInvulnerable(PlayerManager.instance.timeInvulnerable);
        spawnedController.SetFalling();
        allControllers.Add(spawnedController);
        FindObjectOfType<NavMeshCameraController>().toFollow.Add(spawnedController.transform);

        if (dropPlayers)
        {
            StartCoroutine(WaitForPlayerToDrop(spawnedController, endTransform));
        }
        else
        {
            spawnedController.rb.velocity = Vector3.zero;
            spawnedController.rb.angularVelocity = Vector3.zero;
            spawnedController.rb.isKinematic = false;
            spawnedController.SetNotFalling();
            spawnedController.transform.SetPositionAndRotation(endTransform.position, endTransform.rotation);
        }
    }

    public IEnumerator WaitForPlayerToDrop(ControllerMultiPlayer playerDropped, Transform endTransform)
    {
        yield return new WaitForSeconds(dropTime);
        float currentTime = 0f;
        Vector3 startPosition = playerDropped.transform.position;
        Quaternion startRotation = playerDropped.transform.rotation;

        playerDropped.rb.isKinematic = true;

        while (currentTime < dropLerpTime)
        {
            currentTime += Time.deltaTime;

            playerDropped.transform.position = Vector3.Lerp(startPosition, endTransform.position, currentTime / dropLerpTime);
            playerDropped.transform.rotation = Quaternion.Lerp(startRotation, endTransform.rotation, currentTime / dropLerpTime);
            
            yield return null;
        }
        playerDropped.rb.isKinematic = false;
        playerDropped.SetNotFalling();
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
        if (dropPlayers)
        {
            StartCoroutine(WaitForPlayersToDrop());
        }
        else
        {            
            foreach (var i in allControllers)
            {
                i.SetNotFalling();
                i.rb.isKinematic = false;
            }
            for (int i = 0; i < allControllers.Count; i++)
            {
                allControllers[i].transform.SetPositionAndRotation(spawnPoints[i].transform.position, spawnPoints[i].transform.rotation);
            }
            StartLevel();
        }
    }

    public IEnumerator WaitForPlayersToDrop()  
    {
        if (allControllers.Count == 0)
        {
            StartLevel();
            yield break;
        }
        else
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
                i.SetNotFalling();
            }
            StartLevel();
            yield break;
        }
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

    public Transform GetTargetPriority()
    {
        List<ControllerMultiPlayer> randomControllers = allControllers.ToList();
        randomControllers.Shuffle();        
        foreach (var i in randomControllers)
        {
            if (!i.isDead && i.attributes.isPriority)
            {
                return i.transform;                
            }
        }
        return null;
    }

    public Transform GetTarget()
    {
        List<ControllerMultiPlayer> randomControllers = allControllers.ToList();
        randomControllers.Shuffle();
        Transform retTransform = null;
        foreach (var i in randomControllers)
        {
            if (!i.isDead)
            {
                retTransform = i.transform;

                if (i.attributes.isPriority)
                {
                    return retTransform;
                }
            }
        }
        return retTransform;
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public virtual void Pause()
    {
        //GameManager.instance.Pause(); // wont work because of menu event systems
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

