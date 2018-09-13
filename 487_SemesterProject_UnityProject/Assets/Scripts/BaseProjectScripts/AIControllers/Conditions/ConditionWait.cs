using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Conditions/Wait")]
public class ConditionWait : Condition {

    [Header("Settings")]
    public float waitTime;

    public override bool CheckCondition(AI ai)
    {
        return ai.CheckIfCountDownElapsed(waitTime);
    }
}
