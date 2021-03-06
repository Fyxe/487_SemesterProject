﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerChecker : MonoBehaviour
{

    public delegate void PlayerCheck();

    public PlayerCheck playersEmpty;
    public PlayerCheck playersUnempty;    

    [Header("References")]
    public SphereCollider sphereCollider;
    public List<ControllerMultiPlayer> playersInRange = new List<ControllerMultiPlayer>();
    ControllerMultiPlayer cachedPlayer;

    void Awake()
    {
        //playersEmpty = DelegatePlayersEmpty;
        //playersUnempty = DelegatePlayersUnempty;
        sphereCollider.isTrigger = true;
    }

    public void Setup(PlayerCheck empty, PlayerCheck unempty, float radius)
    {
        playersEmpty += empty;
        playersUnempty += unempty;
        sphereCollider.radius = radius;
    }

    public void DelegatePlayersEmpty()
    {

    }

    public void DelegatePlayersUnempty()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        if (!col.isTrigger && (cachedPlayer = col.GetComponentInParent<ControllerMultiPlayer>()) != null && !playersInRange.Contains(cachedPlayer))
        {            
            bool wasEmpty = playersInRange.Count == 0;
            playersInRange.Add(cachedPlayer);         
            if (wasEmpty)
            {
                playersUnempty();
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (!col.isTrigger && (cachedPlayer = col.GetComponentInParent<ControllerMultiPlayer>()) != null && playersInRange.Contains(cachedPlayer))
        {            
            bool wasUnempty = playersInRange.Count > 0;
            playersInRange.Remove(cachedPlayer);    
            if (wasUnempty)
            {
                playersEmpty();
            }
        }
    }
}
