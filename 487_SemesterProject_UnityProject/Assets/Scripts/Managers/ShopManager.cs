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

    private void Awake()
    {
        AudioManager.instance.PlayClipLocalLooped(backgroundMusic);
    }

    protected override void OnFocused()
    {
        base.OnFocused();
        foreach (var i in locationsPositions)
        {
            if (chanceOfSpawningLocaion == 1f || Random.Range(0f,1f) < chanceOfSpawningLocaion)
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
            GameplayManager.instance.GoToNextLevel();
        }
        else
        {
            GameplayManager.instance.EndGame();
        }
    }

    public override void SpawnPlayer(PlayerAttributes newAttributes)
    {
        base.SpawnPlayer(newAttributes);        
        PlayerManager.instance.GetAttributeOfPlayer(newAttributes.indexPlayer).isDead = false;
    }
    
}
