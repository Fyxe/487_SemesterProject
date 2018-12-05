using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameplayManager : SingletonDDOL<GameplayManager>
{
    [Header("Settings")]
    public int piecesMaxIncrease = 2;
    public int piecesMinIncrease = 2;
    [Space]
    public int piecesMaxStart = 5;
    public int piecesMinStart = 5;
    [Space]
    public float enemySpeedMultiplier = 1f;
    public int enemyHealthModifier = 1;
    public float enemySpawnRate = 1f;
    float initialSpeed;
    int initialHealth;
    float initialSpawnRate;
    [Space]
    public float enemySpeedMultiplierChange = 1f;
    public int enemyHealthModifierChange = 1;
    public float enemySpawnRateChange = -0.1f;   

    [Header("References")]
    public int currentLevel = 0;
    public int currentLevelReal = 0;
    public int piecesMaxCurrent;
    public int piecesMinCurrent;

    void Awake()
    {
        initialSpeed = enemySpeedMultiplier;
        initialHealth = enemyHealthModifier;
        initialSpawnRate = enemySpawnRate;
    }

    public void StartGame()
    {
        currentLevel = 0;
        ResetDifficulty();
        currentLevel++;
        LoadSceneManager.instance.LoadScene("Shop");    // TODO tutorial
    }

    public void EndGame()
    {
        ResetDifficulty();
        if (GameManager.instance.dataCurrent.maxDayReached < currentLevel)
        {
            GameManager.instance.dataCurrent.maxDayReached = currentLevel;

        }
        ProgressionManager.instance.OnGameEnd();
        PlayerManager.instance.ResetAllPlayers();
        LoadSceneManager.instance.LoadScene("Menu");
    }

    public void GoToNextLevel()
    {
        currentLevelReal++;
        if (currentLevelReal.IsEven())
        {
            currentLevel++;
            LoadSceneManager.instance.LoadScene("Shop");
        }
        else
        {
            IncreaseDifficulty();
            LoadSceneManager.instance.LoadScene("InLevel");
        }
    }

    public void IncreaseDifficulty()
    {
        piecesMaxCurrent += piecesMaxIncrease;
        piecesMinCurrent += piecesMinIncrease;

        enemyHealthModifier += enemyHealthModifierChange;
        enemySpeedMultiplier += enemySpeedMultiplierChange;
        enemySpawnRate += enemySpawnRateChange;
    }

    public void DecreaseDifficulty()
    {
        piecesMaxCurrent -= piecesMaxIncrease;
        piecesMinCurrent -= piecesMinIncrease;

        enemyHealthModifier -= enemyHealthModifierChange;
        enemySpeedMultiplier -= enemySpeedMultiplierChange;
        enemySpawnRate -= enemySpawnRateChange;
    }

    public void ResetDifficulty()
    {
        piecesMaxCurrent = piecesMaxStart;
        piecesMinCurrent = piecesMinStart;

        enemyHealthModifier = initialHealth;
        enemySpeedMultiplier = initialSpeed;
        enemySpawnRate = initialSpawnRate;
    }
}
