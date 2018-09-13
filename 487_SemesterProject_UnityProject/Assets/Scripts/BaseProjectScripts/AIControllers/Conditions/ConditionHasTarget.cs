using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Conditions/HasTarget")]
public class ConditionHasTarget : Condition
{
    public override bool CheckCondition(AI ai)
    {
        return ai.target != null;
    }
}
