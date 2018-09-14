using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : Singleton<GameDirector>
{

    public bool isMaxStressed = false;
    public float timeMaxStressed = 30f;
    public int stressCurrent = 0;
    public int stressMax = 10000;

    public void AddStress(int amount)
    {
        if (isMaxStressed)
        {
            return;
        }
        stressCurrent += Mathf.Abs(amount);
        CheckStress();
    }

    public void RemoveStress(int amount)
    {
        if (isMaxStressed)
        {
            return;
        }
        stressCurrent -= Mathf.Abs(amount);
        CheckStress();
    }

    void CheckStress()
    {
        if (stressCurrent < 0)
        {
            stressCurrent = 0;
        }
        if (stressCurrent >= stressMax)
        {
            StartCoroutine(HoldMaxStress());
        }
    }

    IEnumerator HoldMaxStress()
    {
        isMaxStressed = true;
        yield return new WaitForSeconds(timeMaxStressed);
        isMaxStressed = false;
        stressCurrent = 0;
    }
}
