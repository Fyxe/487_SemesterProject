using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPosition : MonoBehaviour
{

	public void SpawnObject(Transform toSpawn)
    {
        toSpawn.transform.SetPositionAndRotation(transform.position,transform.rotation);
    }
    
}
