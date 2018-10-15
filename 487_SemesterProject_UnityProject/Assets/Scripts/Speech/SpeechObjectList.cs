using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Speech Data", menuName = "Speech List", order = 51)]
public class SpeechObjectList : ScriptableObject {
    public List<SpeechObject> lineList;

    public SpeechObject Search (int i) {

        SpeechObject result = lineList.Find(n => n.id == i);

        return result;
    }
}
