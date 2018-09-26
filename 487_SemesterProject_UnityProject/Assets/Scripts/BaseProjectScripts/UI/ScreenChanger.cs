using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ScreenChanger : MonoBehaviour
    {
        ScreenBase currentScreen;

        [Header("Screen Changer Settings")]
        public bool waitForOtherTransitions = true;

        void Awake()
        {
            currentScreen = GetComponentInParent<ScreenBase>();
        }

        public void CallbackScreenSet(ScreenBase toChangeTo)
        {
            ScreenManager.instance.ScreenSet(toChangeTo, false, waitForOtherTransitions);
            //ScreenManager.instance.ScreenRemove(currentScreen, waitForOtherTransitions);
            //ScreenManager.instance.ScreenAdd(toChangeTo, waitForOtherTransitions);
        }

        public void CallbackScreenAdd(ScreenBase toAdd)
        {
            ScreenManager.instance.ScreenAdd(toAdd, waitForOtherTransitions);
        }

        public void CallbackScreenRemove(ScreenBase toRemove)
        {
            ScreenManager.instance.ScreenRemove(toRemove, waitForOtherTransitions);
        }

    }
}

