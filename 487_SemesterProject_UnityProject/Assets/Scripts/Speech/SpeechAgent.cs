using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechAgent : MonoBehaviour {

    public SpeechObjectList speechObjectList;
    public Text text; 
    public GUIStyle style;

    //reference to Scriptable Object List
    //speech postion for transform vector
    //reference to manager
    //create object from object pooling manager to display

    private void Start()
    {
        DisplayMessage(0);
    }

    //not listening, but is called in different functions

    public void DisplayMessage(int toDisplayID) {
        SpeechObject line = speechObjectList.Search(toDisplayID);

        //broadcast



        text.text = line.line;
        //display
        if (line.followUp != -1) {
            //bubble for toon to reply

        }
    }
}
