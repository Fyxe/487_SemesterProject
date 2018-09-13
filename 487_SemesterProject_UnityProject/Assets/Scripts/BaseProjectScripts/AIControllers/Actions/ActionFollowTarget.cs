using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/FollowTarget")]
public class ActionFollowTarget : Action
{

    public override void Act(AI ai)
    {
        ai.FollowTarget();
    }

}
