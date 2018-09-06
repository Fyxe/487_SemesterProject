using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class InputControllerClick : MonoBehaviour
{
    // TODO - click only works on certain layers
    // TODO - add pooled object for indicator and make it prefab insertable
    
    NavMeshAgent navmeshAgentPlayer;
    LineRenderer lineRenderer;

    [Header("Click Settings")]
    public bool debugDisplay = true;
    public bool showPath = true;
    public bool enableClickAndHold;
    public float delayPathUpdate = 0.25f;
    float nextPathUpdate = 0f;
    public int clickableLayer;
    public float delayClickAndHold = 0.5f;
    float nextClickAndHold;

    [Header("References")]
    public Indicator clickIndicator;

    void Awake()
    {
        navmeshAgentPlayer = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            nextClickAndHold = Time.time + delayClickAndHold;
            RaycastHit clickedPosition = GetRaycastHitAtMousePosition();

            if (!clickedPosition.Equals(default(RaycastHit)))
            {
                navmeshAgentPlayer.destination = clickedPosition.point;

                Indicator spawnedIndicator = ObjectPoolingManager.instance.CreateObject(clickIndicator, null, clickIndicator.destroyTime) as Indicator;
                spawnedIndicator.gameObject.transform.position = GetRaycastHitAtMousePosition().point;
                spawnedIndicator.Indicate();
            }               

        }
        else if (Input.GetMouseButton(0) && enableClickAndHold && (Time.time > nextClickAndHold))
        {
            if (Time.time > nextPathUpdate)
            {
                nextPathUpdate = Time.time + delayPathUpdate;
                navmeshAgentPlayer.destination = GetRaycastHitAtMousePosition().point;
                Indicator spawnedIndicator = ObjectPoolingManager.instance.CreateObject(clickIndicator, null, clickIndicator.destroyTime) as Indicator;
                spawnedIndicator.gameObject.transform.position = GetRaycastHitAtMousePosition().point;
                spawnedIndicator.Indicate();
            }
        }

        if (debugDisplay)
        {
            DrawDebug();
            DisplayPath();
        }

        if (showPath)
        {
            lineRenderer.positionCount = navmeshAgentPlayer.path.corners.Length;
            lineRenderer.SetPositions(navmeshAgentPlayer.path.corners);
        }
    }

    RaycastHit GetRaycastHitAtMousePosition()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f, 1 << clickableLayer))
        {
            if (DebugManager.instance.debugMousePositionRay && DebugManager.instance.enableDebug)
            {
                Debug.DrawLine(ray.origin, hit.point, Color.green, 1f);
            }
        }
        else if (DebugManager.instance.debugMousePositionRay && DebugManager.instance.enableDebug)
        {
            Debug.DrawRay(ray.origin, ray.direction * 10f, Color.magenta, 1f);
        }

        return hit;
    }

    void DisplayPath()
    {
        for (int i = 0; i < navmeshAgentPlayer.path.corners.Length - 1; i++)
        {
            Debug.DrawLine(navmeshAgentPlayer.path.corners[i], navmeshAgentPlayer.path.corners[i + 1], Color.blue);
        }
    }

    void DrawDebug()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.white);
        }
        else if (debugDisplay)
        {
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
        }
    }

}
