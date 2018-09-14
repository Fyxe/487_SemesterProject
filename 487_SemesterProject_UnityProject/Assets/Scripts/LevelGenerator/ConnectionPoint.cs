using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoint : MonoBehaviour
{    
    public float width
    {
        get
        {
            return Vector3.Distance(transform.position,transformWidth.position);
        }
    }
    public Vector3 direction
    {
        get
        {
            return transformDirection.position - transform.position;
        }
    }
    public ConnectionType type = ConnectionType.ALL;
    public List<ConnectionType> connectableTypes = new List<ConnectionType>();
    public int spawnWeight = 0;

    [Header("References")]
    public Transform transformWidth;
    public Transform transformDirection;
    public ConnectionPoint attachedTo;
    public List<GameObject> prefabSpawnInConnected = new List<GameObject> ();
    public List<GameObject> prefabSpawnInNotConnected = new List<GameObject> ();

    bool connectionSpawned = false;

    public LevelPiece piece;    

    public void Attach(ConnectionPoint newAttachedTo)
    {
        if (attachedTo != null)
        {
            Debug.LogError("This Point is already connected, cannot connect again.");
            return;
        }
        if (newAttachedTo.attachedTo != null)
        {
            Debug.LogError("Connected point is already connected, cannot connect again.");
            return;
        }
        attachedTo = newAttachedTo;
        newAttachedTo.attachedTo = this;
    }

    public void SpawnAtConnection()
    {
        if (attachedTo == null)
        {
            GameObject spawnedPrefabObject = Instantiate(prefabSpawnInConnected.GetRandomValue());
            spawnedPrefabObject.transform.position = transform.position;
            spawnedPrefabObject.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            connectionSpawned = true;
        }
        else
        {
            if (attachedTo.connectionSpawned)
            {
                connectionSpawned = true;
                return;
            }

            bool useMine = true;
            if (spawnWeight > attachedTo.spawnWeight)
            {                
                useMine = true;
            }
            else if (attachedTo.spawnWeight > spawnWeight)
            {                
                useMine = false;
            }
            else
            {
                useMine = Random.Range(0, 2) == 0;
            }

            if (useMine)
            {
                GameObject spawnedPrefabObject = Instantiate(prefabSpawnInConnected.GetRandomValue());
                spawnedPrefabObject.transform.position = transform.position;
                spawnedPrefabObject.transform.rotation = Quaternion.LookRotation(direction,Vector3.up);
            }
            else
            {
                GameObject spawnedPrefabObject = Instantiate(attachedTo.prefabSpawnInConnected.GetRandomValue());
                spawnedPrefabObject.transform.position = attachedTo.transform.position;
                spawnedPrefabObject.transform.rotation = Quaternion.LookRotation(attachedTo.direction, Vector3.up);
            }

            attachedTo.connectionSpawned = true;
            connectionSpawned = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position,0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transformWidth.position, 0.05f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transformDirection.position, 0.05f);
        Gizmos.DrawLine(transform.position, transformDirection.position);
        Gizmos.DrawLine(transform.position, transformWidth.position);
    }

}
