using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/UpdateTarget")]
public class ActionUpdateTarget : Action
{
    public override void Act(AI ai)
    {
        ai.UpdateTarget();
    }
}
