using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentCameraController : MonoBehaviour 
{

    [Header("References")]
    public List<Transform> toFollow = new List<Transform>();
    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.Warp(Vector3.zero);
        agent.updateRotation = false;
    }

    void Update()
    {
        if (toFollow.Count == 0)
        {
            return;
        }

        agent.destination = GetCentralPoint();
        
    }

    public Vector3 GetCentralPoint()
    {
        Vector3 retVec = Vector3.zero;
        foreach (Transform i in toFollow)
        {
            retVec += i.position;
        }
        retVec /= toFollow.Count;
        return retVec;
    }
}
