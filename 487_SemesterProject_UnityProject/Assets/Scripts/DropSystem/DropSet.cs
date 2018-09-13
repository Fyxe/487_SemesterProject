using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Drop Set")]
public class DropSet : ScriptableObject {

    [System.Serializable]
    public class Drop
    {
        public GameObject drop;
        public int weight;
    }

    public List<Drop> allDrops = new List<Drop>();

    public GameObject GetDrop()
    {
        List<GameObject> possibleDrops = new List<GameObject>();
        foreach (var i in allDrops)
        {
            for (int j = 0; j < i.weight; j++)
            {
                possibleDrops.Add(i.drop);
            }
        }
        return possibleDrops.GetRandomValue();
    }
}
