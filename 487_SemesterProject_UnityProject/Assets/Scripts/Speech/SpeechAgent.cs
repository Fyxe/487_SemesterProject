using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpeechAgent : MonoBehaviour {

    public SpeechObjectList speechObjectList;

    public void DisplayMessage(int toDisplayID) {
        SpeechObject line = speechObjectList.Binary(toDisplayID);

        //display
        if (line.followUp != -1) {
            //bubble for toon to reply
            DisplayMessage(line.followUp);
        }
    }
}
