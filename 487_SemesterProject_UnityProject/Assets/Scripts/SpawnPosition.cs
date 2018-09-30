using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPosition : MonoBehaviour {

	public void SpawnObject(Transform toSpawn)
    {
        toSpawn.transform.position = transform.position;
        toSpawn.transform.rotation = transform.rotation;
    }
    
}
