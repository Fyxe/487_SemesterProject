using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitScreenManager : MonoBehaviour
{
    [Header("Settings")]
    public int countPlayer = 1;
    public float lerpTime = 1f;

    [Header("References")]
    public List<Camera> cameras = new List<Camera>();
    public Camera cameraPrimary;
   
    [ContextMenu("Setup Splitscreen")]
    void SetupSplitScreen()
    {        
        int amountTop = 0;      // amount of screens on top
        int amountBottom = 0;   // amount of screens on bottom

        if (cameras.Count % 2 != 0)
        {
            amountTop = ((cameras.Count - 1) / 2) + 1;
            amountBottom = (cameras.Count - 1) / 2;            
        }
        else
        {
            amountTop = cameras.Count / 2;
            amountBottom = amountTop;
        }

        float topWidth = 1f / (float)amountTop;
        float topHeight = 0.5f;
        float botWidth = 1f / (float)amountBottom;
        float botHeight = 0.5f;

        float x = 0f;
        float y = 0f;

        List<Camera> topCameras = cameras.GetRange(0, amountTop);        
        List<Camera> botCameras = cameras.GetRange(amountTop, amountBottom);        

        y = 0.5f;
        x = 0f;
        foreach (var i in topCameras)
        {           
            Rect topRect = new Rect(x, y, topWidth, topHeight);            
            i.rect = topRect;            
            x += topWidth;
            i.depth = -1;
        }

        y = 0.0f;
        x = 0f;
        foreach (var i in botCameras)
        {
            Rect botRect = new Rect(x, y, botWidth, botHeight);
            i.rect = botRect;
            x += botWidth;
            i.depth = -1;
        }        
    }

    [ContextMenu("Setup Singlescreen")]
    void SetupPrimaryScreen()
    {
        cameraPrimary.rect = new Rect(0, 0, 1, 1);
        cameraPrimary.depth = float.MaxValue;
    }
    
}