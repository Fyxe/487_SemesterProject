using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="PluggableAI/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public float moveSpeed = 1;
    public float lookRange = 40f;
    public float lookSphereCastRadius = 1f;

    public float attackRange = 5f;
    public float attackDelay = 1f;
    public float attackForce = 1f;
    public int attackDamage = 50;

    public float searchDuration = 4f;
    public float searchingTurnSpeed = 120f;

    public LayerMask whatToHit;
}
