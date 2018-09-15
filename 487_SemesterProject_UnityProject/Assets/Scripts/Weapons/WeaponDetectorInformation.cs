using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class WeaponDetectorInformation : MonoBehaviour
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
            UpdateInformation();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (!col.isTrigger && (cachedPlayer = col.GetComponentInParent<ControllerMultiPlayer>()) != null && playersInRange.Contains(cachedPlayer))
        {
            playersInRange.Remove(cachedPlayer);
            UpdateInformation();
        }
    }

    void UpdateInformation()
    {
        if (playersInRange.Count > 0)
        {
            weapon.EnableInformation();
        }
        else
        {
            weapon.DisableInformation();
        }
    }

    public void SetRange(float newRange)
    {
        if (sphereCollider == null)
        {
            sphereCollider = GetComponent<SphereCollider>();
        }
        sphereCollider.radius = newRange;
    }
}
