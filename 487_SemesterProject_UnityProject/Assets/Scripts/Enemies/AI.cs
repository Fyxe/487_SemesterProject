using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : Damageable
{
    //[Header("Enemy Settings")]

    [Header("Enemy References")]
    public State stateCurrent;
    public State stateRemain;

    public float timeElapsedInState;

    public Transform target;

    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();   
    }

    void Update()
    {
        timeElapsedInState += Time.deltaTime;
        stateCurrent.UpdateState(this);
    }

    public bool CheckIfCountDownElapsed(float duration)
    {
        return (timeElapsedInState >= duration);
    }

    void OnExitState()
    {
        timeElapsedInState = 0;
    }

    public void ChangeState(State newState)
    {
        if (newState != stateRemain)
        {
            OnExitState();
            stateCurrent = newState;            
        }
    }

    public void FollowTarget()
    {
        if (target != null)
        {
            agent.destination = target.position;
        }        
        else
        {
            Debug.LogWarning("AI has no target to follow.");
        }
    }

    public bool GetTarget()
    {
        target = LevelManager.instance.GetTarget();
        return target == null;
    }
}
