using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Attack")]
public class AttackAction : Action
{
    public override void Act(StateController stateController)
    {
        if (Time.time >= stateController.nextAttackTime)
        {
            stateController.nextAttackTime = Time.time + stateController.enemyStats.attackDelay;
            Attack(stateController);
        }
    }

    private void Attack(StateController stateController)
    {
        RaycastHit hit;

        Debug.DrawRay(stateController.eyes.position, stateController.eyes.forward.normalized * stateController.enemyStats.attackRange, Color.red);
        
        if (Physics.SphereCast(stateController.eyes.position, stateController.enemyStats.lookSphereCastRadius, stateController.eyes.forward, out hit, stateController.enemyStats.attackRange, stateController.enemyStats.whatToHit))
        {
            ("Enemy Attacking Super Buff Mode").Log();
            stateController.gameObject.transform.localScale = Vector3.one * 10;
        }
    }
}