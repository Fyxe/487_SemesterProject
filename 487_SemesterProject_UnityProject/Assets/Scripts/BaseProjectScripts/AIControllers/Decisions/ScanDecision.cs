using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Scan")]
public class ScanDecision : Decision
{
    public override bool Decide(StateController stateController)
    {
        bool noEnemyInSight = Scan(stateController);
        return noEnemyInSight;
    }

    private bool Scan(StateController stateController)
    {
        stateController.navMeshAgent.isStopped = true;
        stateController.transform.Rotate(0, stateController.enemyStats.searchingTurnSpeed * Time.deltaTime, 0);
        return stateController.CheckIfCountDownElapsed(stateController.enemyStats.searchDuration);
    }
}