using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpeechObject {
    [SerializeField]
    public int id;
    [SerializeField]
    public string line;
    [SerializeField]
    public int followUp;

    public int idTag {
        get {
            return id;
        }
    }
    public string lineDesc {
        get {
            return line;
        }
    }
    public int followUpTag {
        get {
            return followUp;
        }
    }
}
