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
            generator.GenerateLevelDFS();
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

}
