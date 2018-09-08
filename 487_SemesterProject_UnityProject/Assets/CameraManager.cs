using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Settings")]
    public int countPlayer = 1;
    public float lerpTime = 1f;

    [Header("References")]
    public List<Camera> cameras = new List<Camera>();
    public Camera cameraCurrent;
    
    void Awake()
    {
        //SetupCameras();
    }
        
    [ContextMenu("SetupCameras")]
    void SetupCameras()
    {
        int count = cameras.Count;

        int amountTop = 0;      // amount of screens on top
        int amountBottom = 0;   // amount of screens on bottom

        if (count % 2 != 0)
        {
            amountTop = ((count - 1) / 2) + 1;
            amountBottom = (count - 1) / 2;            
        }
        else
        {
            amountTop = count / 2;
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

        List<Rect> rects = new List<Rect>();

        y = 0.5f;
        x = 0f;
        float topXIterator = 1f / (float)amountTop;
        foreach (var i in topCameras)
        {           
            Rect topRect = new Rect(x, y, topWidth, topHeight);            
            //i.rect = topRect;            
            x += topXIterator;
            rects.Add(topRect);
        }

        y = 0.0f;
        x = 0f;
        float bottomXIterator = 1f / (float)amountBottom;
        foreach (var i in botCameras)
        {
            Rect botRect = new Rect(x, y, botWidth, botHeight);
            //i.rect = botRect;
            x += bottomXIterator;
            rects.Add(botRect);
        }
        StartCoroutine(SetCameraDefaultPositions(rects));
    }

    IEnumerator SetCameraDefaultPositions(List<Rect> endRects)
    {
        if (endRects.Count != cameras.Count)
        {
            yield break;
        }
        List<Rect> startRects = new List<Rect>();
        foreach (var i in cameras)
        {
            startRects.Add(i.rect);
        }

        float currentTime = 0f;
        while (currentTime < lerpTime)
        {
            currentTime += Time.deltaTime;
            for (int i = 0; i < cameras.Count; i++)
            {
                cameras[i].rect = new Rect(
                    Mathf.Lerp(startRects[i].x, endRects[i].x,currentTime / lerpTime),
                    Mathf.Lerp(startRects[i].y, endRects[i].y, currentTime / lerpTime),
                    Mathf.Lerp(startRects[i].width, endRects[i].width, currentTime / lerpTime),
                    Mathf.Lerp(startRects[i].height, endRects[i].height, currentTime / lerpTime)
                    );
            }
            yield return null;
        }        
    }

    [ContextMenu("Teest")]
    void Test()
    {
        StartCoroutine(ViewOneCamera(cameras[0]));
    }

    IEnumerator ViewOneCamera(Camera toView)
    {
        Rect startRect = toView.rect;
        Rect endRect = new Rect(0,0,1,1);

        float currentTime = 0f;
        while (currentTime < lerpTime)
        {
            currentTime += Time.deltaTime;
            toView.rect = new Rect(
                    Mathf.Lerp(startRect.x, endRect.x, currentTime / lerpTime),
                    Mathf.Lerp(startRect.y, endRect.y, currentTime / lerpTime),
                    Mathf.Lerp(startRect.width, endRect.width, currentTime / lerpTime),
                    Mathf.Lerp(startRect.height, endRect.height, currentTime / lerpTime)
                    );
            yield return null;
        }
    }
    
}