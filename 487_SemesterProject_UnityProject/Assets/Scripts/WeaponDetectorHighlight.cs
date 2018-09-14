using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class WeaponDetectorHighlight : MonoBehaviour
{

    // [Header("References")]
    Weapon weapon;
    SphereCollider sphereCollider;

    List<ControllerMultiPlayer> playersInRange = new List<ControllerMultiPlayer>();
    ControllerMultiPlayer cachedPlayer;

    void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        weapon = GetComponentInParent<Weapon>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (!col.isTrigger && (cachedPlayer = col.GetComponentInParent<ControllerMultiPlayer>()) != null && !playersInRange.Contains(cachedPlayer))
        {
            playersInRange.Add(cachedPlayer);
            UpdateHilight();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (!col.isTrigger && (cachedPlayer = col.GetComponentInParent<ControllerMultiPlayer>()) != null && playersInRange.Contains(cachedPlayer))
        {
            playersInRange.Remove(cachedPlayer);
            UpdateHilight();
        }
    }

    void UpdateHilight()
    {
        if (playersInRange.Count > 0)
        {
            weapon.EnableHighlight();
        }
        else
        {
            weapon.DisableHighlight();
        }
    }

    public void SetRange(float newRange)
    {
        sphereCollider.radius = newRange;
    }
}
