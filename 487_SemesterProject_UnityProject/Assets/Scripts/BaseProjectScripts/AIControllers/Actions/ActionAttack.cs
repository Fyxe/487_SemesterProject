using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Attack")]
public class ActionAttack : Action
{
    public override void Act(AI ai)
    {
        ai.AttemptAttack();
    }
}
