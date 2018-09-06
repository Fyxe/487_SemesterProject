using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ScreenChanger : MonoBehaviour
    {
        Screen currentScreen;

        [Header("Screen Changer Settings")]
        public bool waitForOtherTransitions = true;

        void Awake()
        {
            currentScreen = GetComponentInParent<Screen>();
        }

        public void CallbackScreenSet(Screen toChangeTo)
        {
            ScreenManager.instance.ScreenSet(toChangeTo, false, waitForOtherTransitions);
            //ScreenManager.instance.ScreenRemove(currentScreen, waitForOtherTransitions);
            //ScreenManager.instance.ScreenAdd(toChangeTo, waitForOtherTransitions);
        }

        public void CallbackScreenAdd(Screen toAdd)
        {
            ScreenManager.instance.ScreenAdd(toAdd, waitForOtherTransitions);
        }

        public void CallbackScreenRemove(Screen toRemove)
        {
            ScreenManager.instance.ScreenRemove(toRemove, waitForOtherTransitions);
        }

    }
}

