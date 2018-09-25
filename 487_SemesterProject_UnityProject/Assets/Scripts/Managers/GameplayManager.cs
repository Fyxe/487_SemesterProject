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

    [Header("References")]
    public int currentLevel = 0;
    public int piecesMaxCurrent;
    public int piecesMinCurrent;    


    public void StartGame()
    {
        ResetDifficulty();
        LoadSceneManager.instance.LoadScene("Shop");    // TODO tutorial
    }

    public void EndGame()
    {
        ResetDifficulty();
        ProgressionManager.instance.OnGameEnd();
        PlayerManager.instance.ResetAllPlayers();
        LoadSceneManager.instance.LoadScene("Menu");
    }

    public void GoToNextLevel()
    {
        currentLevel++;
        if (currentLevel.IsEven())
        {
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
    }

    public void DecreaseDifficulty()
    {
        piecesMaxCurrent -= piecesMaxIncrease;
        piecesMinCurrent -= piecesMinIncrease;
    }

    public void ResetDifficulty()
    {
        piecesMaxCurrent = piecesMaxStart;
        piecesMinCurrent = piecesMinStart;
    }
}
