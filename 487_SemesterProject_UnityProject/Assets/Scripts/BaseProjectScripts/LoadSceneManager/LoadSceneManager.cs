using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSceneManager : Singleton<LoadSceneManager>
{

    public enum ScreenTransitionType
    {
        NONE, fade //, wipe
    }

    [Header("Load Scene Manager")]
    public ScreenTransitionType type = ScreenTransitionType.fade;
    bool isWaitingOnLoad = false;
    bool isTransitioning = false;
    public float timeTransition = 0.4f;
    public float timeLoadingMin = 1f;

    [Header("References")]
    public Image wipeImage;

    protected override void Initialize()
    {
        name = "LoadSceneManager";
        SceneManager.sceneLoaded += DoneWaitingOnLoad;
    }

    /// <summary>
    /// Sets the internal boolean of waiting to false which allows the transitions to fade back in.
    /// </summary>
    /// <param name="scene">The scene loaded in.</param>
    /// <param name="mode">The mode the scene was loaded by.</param>
    public void DoneWaitingOnLoad(Scene scene, LoadSceneMode mode)
    {
        if (DebugManager.instance.enableDebug && DebugManager.instance.debugFinishedLoadingSceneName)
        {
            Debug.Log("DoneWaitingOnLoad(" + scene.name + ", " + mode.ToString() + ")");
        }
        if (mode == LoadSceneMode.Additive)
        {

        }
        else
        {
            
        }
        isWaitingOnLoad = false;
    }

    /// <summary>
    /// Attempts to load a scene. Used for button calls only.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void CallbackLoadScene(string sceneName)
    {
        LoadScene(sceneName);
    }

    /// <summary>
    /// Attempts to load a scene. If the scene is already transitioning it returns false.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    /// <param name="mode">The mode that the scene should be loaded in.</param>
    /// <param name="fadeIn">The transition type of the fading in function.</param>
    /// <param name="fadeOut">The transition type of the fading out function.</param>
    /// <returns>Returns false if the scene does not attempt to load.</returns>
    public bool LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, ScreenTransitionType fadeIn = ScreenTransitionType.NONE, ScreenTransitionType fadeOut = ScreenTransitionType.NONE)
    {
        if (isTransitioning)
        {
            if (DebugManager.instance.enableDebug && DebugManager.instance.debugSceneNameWhenCalled)
            {
                Debug.Log("Already transitioning to " + sceneName + ". Cannot transition.");
            }
            return false;
        }
        else
        {
            if (fadeIn == ScreenTransitionType.NONE)
            {
                fadeIn = type;
            }
            if (fadeOut == ScreenTransitionType.NONE)
            {
                fadeOut = type;
            }
            if (DebugManager.instance.enableDebug && DebugManager.instance.debugSceneNameWhenCalled)
            {
                Debug.Log("Loading scene: " + sceneName);
            }
            StartCoroutine(LoadSceneCoroutine(sceneName, mode, fadeIn, fadeOut));
            return true;
        }
    }

    IEnumerator LoadSceneCoroutine(string sceneName, LoadSceneMode mode, ScreenTransitionType fadeIn, ScreenTransitionType fadeOut)
    {
        isTransitioning = true;
        switch (fadeIn)
        {
            case ScreenTransitionType.fade:
                yield return StartCoroutine(FadeCoroutine(true));
                break;
        }

        isWaitingOnLoad = true;
               
        SceneManager.LoadSceneAsync("LoadingScene");   // loads the loading scene

        while (isWaitingOnLoad)
        {
            yield return null;
        }

        switch (fadeOut)
        {
            case ScreenTransitionType.fade:
                yield return StartCoroutine(FadeCoroutine(false));
                break;
        }
        float startTime = Time.time;

        isWaitingOnLoad = true;

        LoadingSceneManager loading = FindObjectOfType<LoadingSceneManager>();

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive); // Loads the current scene    

        yield return operation;

        float timeRemaining = timeLoadingMin - (Time.time - startTime);
        if (timeRemaining > 0)
        {
            yield return new WaitForSeconds(timeRemaining);
        }

        if (loading != null)
        {
            loading.TrackProgress(operation);
        }
        else
        {
            Debug.LogError("No loading scene manager was found. Progress bar will not increase.");
        }

        switch (fadeIn)
        {
            case ScreenTransitionType.fade:
                yield return StartCoroutine(FadeCoroutine(true));
                break;
        }

        SceneManager.UnloadSceneAsync("LoadingScene");

        while (isWaitingOnLoad)
        {
            yield return null;
        }

        switch (fadeOut)
        {
            case ScreenTransitionType.fade:
                yield return StartCoroutine(FadeCoroutine(false));
                break;
        }

        isTransitioning = false;
    }

    // ======================================================================================================
    // Transition Coroutines
    // ======================================================================================================

    /// <summary>
    /// Uses the canvas attached to the load scene manager to fade in/out an image by it's alpha.
    /// </summary>
    /// <param name="fadingIn">Is the transition fading in or out?</param>
    /// <returns></returns>
    public IEnumerator FadeCoroutine(bool fadingIn)
    {
        Color startColor = wipeImage.color;
        Color endColor = wipeImage.color;

        if (fadingIn)
        {
            wipeImage.gameObject.SetActive(true);
            startColor.a = 0f;
            endColor.a = 1f;
        }
        else
        {
            startColor.a = 1f;
            endColor.a = 0f;
        }

        float currentTime = 0f;
        while (currentTime < timeTransition)
        {
            currentTime += Time.deltaTime;
            wipeImage.color = Color.Lerp(startColor, endColor, currentTime / timeTransition);
            yield return null;
        }

        if (!fadingIn)
        {
            wipeImage.gameObject.SetActive(false);
        }
    }
}
