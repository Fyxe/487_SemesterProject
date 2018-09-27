﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : Damageable
{
    [Header("Enemy Settings")]
    public int aiID = -1;
    public string aiName;
    public string aiDescription;
    public Sprite aiSprite;
    public int damage = 1;
    public float speedMove = 3f;
    public float radiusPlayerDetection = 3f;
    public float delayUpdateTarget = 0.1f;
    public int pointsOnHit = 0;
    public int pointsOnDeath = 10;
    float nextUpdateTarget = 0;
    public float delayAttack = 1f;
    float nextAttack = 0f;

    [Header("Enemy References")]
    public State stateCurrent;
    
    public float timeElapsedInState = 0;

    public Transform target;
    
    public NavMeshAgent agent;
    ControllerMultiPlayer cachedPlayer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        timeElapsedInState = 0;
    }

    void Update()
    {
        if (LevelManager.instance.isPlaying)
        {
            timeElapsedInState += Time.deltaTime;
            agent.enabled = true;
            stateCurrent.UpdateState(this);            
        }
        else
        {
            agent.enabled = false;
        }
    }

    public bool CheckIfCountDownElapsed(float duration)
    {
        bool retBool = (timeElapsedInState >= duration);
        //Debug.Log(timeElapsedInState + " : " + duration);
        return retBool;
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

    public void AttemptUpdateTarget()
    {
        if (Time.time > nextUpdateTarget)
        {
            nextUpdateTarget = Time.time + delayUpdateTarget;
            UpdateTarget();
        }
    }

    protected virtual void UpdateTarget()
    {        
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

    public void AttemptAttack()
    {
        if (Time.time > nextAttack)
        {
            nextAttack = Time.time + delayAttack;
            Attack();
        }
    }

    protected virtual void Attack()
    {

    }

    public override void OnHurt()
    {
        base.OnHurt();
        ProgressionManager.instance.scoreCurrentInLevel += pointsOnHit;
    }

    public override void OnDeath()
    {
        base.OnDeath();
        ProgressionManager.instance.scoreCurrentInLevel += pointsOnDeath;
        if (GameLevelManager.instance is GameLevelManager)
        {
            (GameLevelManager.instance as GameLevelManager).allAI[pool.objectPrefab as AI].Remove(this);
        }
        DestroyThisObject();
    }

}
