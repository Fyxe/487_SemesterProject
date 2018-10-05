using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpeechAgent : MonoBehaviour {

    public SpeechObjectList speechObjectList;
    public GUIStyle style;

    private void Start()
    {
        DisplayMessage(0);
    }

    public void DisplayMessage(int toDisplayID) {
        SpeechObject line = speechObjectList.Search(toDisplayID);

        SpeechText text = new SpeechText();
        text.setText(line.line, 3000, style);

        //display
        if (line.followUp != -1) {
            //bubble for toon to reply

        }
    }
}
