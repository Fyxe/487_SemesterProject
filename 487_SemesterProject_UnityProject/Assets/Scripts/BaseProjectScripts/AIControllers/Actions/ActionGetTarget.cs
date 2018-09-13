using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/GetTarget")]
public class ActionGetTarget : Action
{

    public override void Act(AI ai)
    {
        ai.GetTarget();
    }

}
