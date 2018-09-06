using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Look")]
public class LookDecision : Decision
{

    public override bool Decide(StateController stateController)
    {
        bool targetVisible = Look(stateController);
        return targetVisible;
    }

    private bool Look(StateController stateController)
    {
        RaycastHit hit;

        Debug.DrawRay(stateController.eyes.position, stateController.eyes.forward.normalized * stateController.enemyStats.lookRange, Color.green);

        if (Physics.SphereCast(stateController.eyes.position, stateController.enemyStats.lookSphereCastRadius, stateController.eyes.forward, out hit, stateController.enemyStats.lookRange, stateController.enemyStats.whatToHit))
        {
            stateController.chaseTarget = hit.transform;
            return true;
        }
        else
        {
            return false;
        }
    }
}
