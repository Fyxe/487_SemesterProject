using System.Collections;
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

    [HideInInspector] public Rigidbody rb;

    List<ControllerMultiPlayer> playersInRange = new List<ControllerMultiPlayer>();

    public AudioClip deathSound;
    public AudioClip hurtSound;
    public AudioClip attackSound;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        timeElapsedInState = 0;
    }

    void Update()
    {
        if (LevelManager.instance.isPlaying)
        {
            agent.speed = speedMove * GameplayManager.instance.enemySpeedMultiplier;
            timeElapsedInState += Time.deltaTime;
            agent.enabled = true;
            stateCurrent.UpdateState(this);   
            if (Time.time > nextAttack)
            {
                nextAttack = Time.time + delayAttack;
                foreach (var i in playersInRange)
                {
                    i.Hurt(1);
                }
            }
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
        Transform targetPriority = LevelManager.instance.GetTargetPriority();
        if (targetPriority != null)
        {
            target = targetPriority;
            return;
        }

        List<ControllerMultiPlayer> playersFound = new List<ControllerMultiPlayer>();

        int layer = 1 << LayerMask.NameToLayer("Player");
        Collider[] cols = Physics.OverlapSphere(transform.position, radiusPlayerDetection, layer);
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

    private void OnTriggerEnter(Collider col)
    {
        if (!col.isTrigger && (cachedPlayer = col.GetComponentInParent<ControllerMultiPlayer>()) != null && !playersInRange.Contains(cachedPlayer) && GetComponentInParent<Weapon>() == null)
        {
            playersInRange.Add(cachedPlayer);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (!col.isTrigger && (cachedPlayer = col.GetComponentInParent<ControllerMultiPlayer>()) != null && playersInRange.Contains(cachedPlayer))
        {
            playersInRange.Remove(cachedPlayer);
        }
    }
}
