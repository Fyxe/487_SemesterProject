using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechText : PooledObject {

    private bool isDisplayed;
    string text;
    int time;

    GUIStyle style;

    public void setText (string _text, int _time, GUIStyle _style) {
        text = _text;
        time = _time;
        style = _style;
        isDisplayed = true;
        Invoke("Stop", time);
    }

    private void Stop () {
        isDisplayed = false;
    }

    private void OnGUI() {
        if (isDisplayed == true) {
            GUI.Label(new Rect(10, Screen.height - 100, 0, 0), text, style);
        }
    }



}
