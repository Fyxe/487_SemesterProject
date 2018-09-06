using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneManager : Singleton<LoadingSceneManager> {
    
    public Image loadingBar;

    public void TrackProgress(AsyncOperation operation)
    {
        StartCoroutine(TrackProgressCoroutine(operation));
    }

    IEnumerator TrackProgressCoroutine(AsyncOperation operation)
    {
        while (!operation.isDone)
        {
            if (loadingBar == null)
            {
                yield break;
            }
            loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, operation.progress,0.2f); // TODO refine this lerp
            yield return null;
        }
        loadingBar.fillAmount = 1f;
    }
}
