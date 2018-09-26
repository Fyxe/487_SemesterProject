using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ScreenManager : Singleton<ScreenManager>
    {

        [Header("Settings")]
        public float delayMultipleScreenTransitionIn = 1f;
        public float delayMultipleScreenTransitionOut = 1f;
        public int screensTransitioning = 0;
        public bool aScreenIsTransitioning
        {
            get
            {
                return screensTransitioning > 0;
            }
        }

        [Header("References")]
        public List<ScreenBase> currentlyDisplayedScreens = new List<ScreenBase>();
        public ScreenBase debugStartOnThisScreen;

        void Start()
        {
            ScreenAdd(debugStartOnThisScreen, false);
        }

        public void Setup(ScreenBase initialSetupScreen)
        {

        }

        public void ScreenSet(ScreenBase toSet, bool useDelay, bool waitForOtherTransitions)
        {
            ScreenRemoveAll(useDelay,waitForOtherTransitions);
            ScreenAdd(toSet,waitForOtherTransitions);
        }

        public void ScreenAdd(ScreenBase toAdd, bool waitForOtherTransitions)
        {
            StartCoroutine(ScreenChangeCoroutine(new List<ScreenBase> { toAdd }, false, waitForOtherTransitions, true));
        }

        public void ScreenRemoveAll(bool useDelay, bool waitForOtherTransitions)
        {
            ScreenMultipleRemove(currentlyDisplayedScreens, useDelay, waitForOtherTransitions);
        }

        public void ScreenRemove(ScreenBase toRemove, bool waitForOtherTransitions)
        {
            StartCoroutine(ScreenChangeCoroutine(new List<ScreenBase> { toRemove }, false, waitForOtherTransitions, false));
        }

        public void ScreenMultipleAdd(List<ScreenBase> toAdd, bool useDelay, bool waitForOtherTransitions)
        {
            StartCoroutine(ScreenChangeCoroutine(toAdd, useDelay, waitForOtherTransitions, true));
        }

        public void ScreenMultipleRemove(List<ScreenBase> toRemove, bool useDelay, bool waitForOtherTransitions)
        {
            StartCoroutine(ScreenChangeCoroutine(toRemove, useDelay, waitForOtherTransitions, false));
        }

        IEnumerator ScreenChangeCoroutine(List<ScreenBase> toChange, bool useDelay, bool waitForOtherTransitions, bool isAdding)
        {
            List<ScreenBase> screensToTransition = toChange.ToList();

            foreach (ScreenBase i in screensToTransition)
            {
                if (i == null)
                {
                    Debug.LogError("Screen given is null");
                    continue;
                }

                if (waitForOtherTransitions)
                {
                    while (aScreenIsTransitioning)
                    {
                        yield return null;
                    }
                }

                screensTransitioning++;
                if (isAdding)
                {
                    currentlyDisplayedScreens.Remove(i);
                    yield return StartCoroutine(i.TransitionIn());
                    i.OnTransitionedIn();
                    currentlyDisplayedScreens.Add(i);
                    if (useDelay)
                    {
                        yield return new WaitForSeconds(delayMultipleScreenTransitionIn);
                    }
                }
                else
                {
                    currentlyDisplayedScreens.Remove(i);
                    yield return StartCoroutine(i.TransitionOut());
                    i.OnTransitionedOut();                    
                    if (useDelay)
                    {
                        yield return new WaitForSeconds(delayMultipleScreenTransitionOut);
                    }
                }
                screensTransitioning--;
            }
        }

        public void CallbackChangeScene(string sceneName)
        {
            LoadSceneManager.instance.LoadScene(sceneName);
        }
    }
}
