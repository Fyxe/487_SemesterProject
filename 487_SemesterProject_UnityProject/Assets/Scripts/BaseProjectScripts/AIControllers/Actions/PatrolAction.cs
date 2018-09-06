using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Patrol")]
public class PatrolAction : Action
{
    public override void Act(StateController stateController)
    {
        Patrol(stateController);
    }

    private void Patrol(StateController stateController)
    {
        stateController.navMeshAgent.destination = stateController.wayPointList[stateController.nextWayPoint].position;
        stateController.navMeshAgent.isStopped = false;

        if (stateController.navMeshAgent.remainingDistance <= stateController.navMeshAgent.stoppingDistance && !stateController.navMeshAgent.pathPending)
        {
            stateController.nextWayPoint = (stateController.nextWayPoint + 1) % stateController.wayPointList.Count;
        }
    }
}