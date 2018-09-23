using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : Singleton<GameDirector>
{
    // TODO mob random spawn time

    public enum DirectorMode { climbing, relaxing, maxStressed }
    public DirectorMode mode = DirectorMode.climbing;
    public float timeMaxStressed = 30f;
    public float timeRelaxing = 10f;
    public int stressCurrent = 0;
    public int stressMax = 10000;    

    public void AddStress(int amount)
    {
        if (mode != DirectorMode.climbing)
        {
            return;
        }
        stressCurrent += Mathf.Abs(amount);
        CheckStress();
    }

    public void RemoveStress(int amount)
    {
        if (mode != DirectorMode.climbing)
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
        mode = DirectorMode.maxStressed;
        yield return new WaitForSeconds(timeMaxStressed);
        mode = DirectorMode.relaxing;
        yield return new WaitForSeconds(timeRelaxing);
        mode = DirectorMode.climbing;
        stressCurrent = 0;
        yield break;
    }
}
