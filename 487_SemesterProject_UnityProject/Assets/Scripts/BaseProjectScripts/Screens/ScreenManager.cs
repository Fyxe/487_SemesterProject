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
        public List<Screen> currentlyDisplayedScreens = new List<Screen>();
        public Screen debugStartOnThisScreen;

        void Start()
        {
            ScreenAdd(debugStartOnThisScreen, false);
        }

        public void Setup(Screen initialSetupScreen)
        {

        }

        public void ScreenSet(Screen toSet, bool useDelay, bool waitForOtherTransitions)
        {
            ScreenRemoveAll(useDelay,waitForOtherTransitions);
            ScreenAdd(toSet,waitForOtherTransitions);
        }

        public void ScreenAdd(Screen toAdd, bool waitForOtherTransitions)
        {
            StartCoroutine(ScreenChangeCoroutine(new List<Screen> { toAdd }, false, waitForOtherTransitions, true));
        }

        public void ScreenRemoveAll(bool useDelay, bool waitForOtherTransitions)
        {
            ScreenMultipleRemove(currentlyDisplayedScreens, useDelay, waitForOtherTransitions);
        }

        public void ScreenRemove(Screen toRemove, bool waitForOtherTransitions)
        {
            StartCoroutine(ScreenChangeCoroutine(new List<Screen> { toRemove }, false, waitForOtherTransitions, false));
        }

        public void ScreenMultipleAdd(List<Screen> toAdd, bool useDelay, bool waitForOtherTransitions)
        {
            StartCoroutine(ScreenChangeCoroutine(toAdd, useDelay, waitForOtherTransitions, true));
        }

        public void ScreenMultipleRemove(List<Screen> toRemove, bool useDelay, bool waitForOtherTransitions)
        {
            StartCoroutine(ScreenChangeCoroutine(toRemove, useDelay, waitForOtherTransitions, false));
        }

        IEnumerator ScreenChangeCoroutine(List<Screen> toChange, bool useDelay, bool waitForOtherTransitions, bool isAdding)
        {
            List<Screen> screensToTransition = toChange.ToList();

            foreach (Screen i in screensToTransition)
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
