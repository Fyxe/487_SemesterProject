using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/ActiveState")]
public class ActiveStateDecision : Decision
{
    public override bool Decide(StateController stateController)
    {
        bool chaseTargetIsActive = stateController.chaseTarget.gameObject.activeSelf;
        return chaseTargetIsActive;
    }
}