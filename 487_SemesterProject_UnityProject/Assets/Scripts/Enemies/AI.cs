using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : Damageable
{
    [Header("Enemy Settings")]
    public float radiusPlayerDetection = 3f;
    public float delayUpdateTarget = 0.1f;
    float nextUpdateTarget = 0;

    [Header("Enemy References")]
    public State stateCurrent;
    
    public float timeElapsedInState = 0;

    public Transform target;

    NavMeshAgent agent;
    ControllerMultiPlayer cachedPlayer;

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
        if (newState != null)
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

    public void UpdateTarget()
    {
        if (Time.time > nextUpdateTarget)
        {
            nextUpdateTarget = Time.time + delayUpdateTarget;

            List<ControllerMultiPlayer> playersFound = new List<ControllerMultiPlayer>();

            Collider[] cols = Physics.OverlapSphere(transform.position, radiusPlayerDetection, LayerMask.NameToLayer("Player"));
            foreach (var i in cols)
            {
                if (!i.isTrigger && (cachedPlayer = i.GetComponentInParent<ControllerMultiPlayer>()) != null && !playersFound.Contains(cachedPlayer))
                {
                    playersFound.Add(cachedPlayer);
                }
            }
            float dist = float.MaxValue;
            foreach (var i in playersFound)
            {
                float newDist = Vector3.Distance(transform.position, i.transform.position);
                if (newDist < dist)
                {
                    dist = newDist;
                    target = i.transform;
                }
            }
        }
    }

}
