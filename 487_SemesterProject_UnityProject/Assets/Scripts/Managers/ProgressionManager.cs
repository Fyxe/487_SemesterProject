using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager : Singleton<ProgressionManager>
{
    [System.Serializable]
    public class ProgressionLevel
    {
        public int requiredXp = 0;
        public List<DropSet> dropSetsUnlocked = new List<DropSet> ();
        // other unlocks go here
    }


    [Header("Settings")]
    public List<ProgressionLevel> levels = new List<ProgressionLevel>();

    [Header("References")]
    public int currentGameLevel = -1;
    int m_currentGameExperience = 0;
    public int currentGameExperience
    {
        get
        {
            return m_currentGameExperience;
        }
        set
        {
            m_currentGameExperience = value;
            CheckLevel();
        }
    }
    public int currentScore = 0;    

    void Awake()
    {
        levels = levels.OrderBy(x => x.requiredXp).ToList();
    }

    public bool SetLevel(int newLevel)
    {
        if (newLevel >= levels.Count || newLevel == -1)
        {
            return false;
        }
        else
        {
            currentGameLevel = newLevel;
            currentGameExperience = levels[currentGameLevel].requiredXp;
            for (int i = 0; i < currentGameLevel; i++)
            {
                DropManager.instance.AddDropSetsToMaster(levels[i].dropSetsUnlocked);
                // other unlocks go here
            }
            return true;
        }
    }

    void CheckLevel()
    {
        if (currentGameLevel + 1 < levels.Count && currentGameExperience >= levels[currentGameLevel + 1].requiredXp)
        {
            DropManager.instance.AddDropSetsToMaster(levels[currentGameLevel + 1].dropSetsUnlocked);
            // other unlocks go here
            currentGameLevel++;
        }
    }

    public void OnGameEnd()
    {
        currentGameExperience += currentScore;
        currentScore = 0;
    }

    public float GetPercentageToNextLevel()
    {
        if (currentGameLevel + 1 < levels.Count)
        {
            float numerator = 0;
            if (currentGameLevel >= 0)
            {
                numerator = levels[currentGameLevel].requiredXp - currentGameExperience;
            }
            
            float denominator = levels[currentGameLevel + 1].requiredXp - currentGameExperience;
            return numerator / denominator;
        }
        else
        {
            return 1f;
        }
    }

}
