using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ScreenBase : MonoBehaviour
    {
        //[Header("Settings")]

        public delegate void Transition();

        public Transition transitionIn;
        public Transition transitionOut;

        void Awake()
        {
            transitionIn = DelegateTransitionIn;
            transitionOut = DelegateTransitionOut;
        }

        public virtual IEnumerator TransitionIn()
        {            
            yield break;
        }

        public virtual IEnumerator TransitionOut()
        {            
            yield break;
        }

        public virtual void OnTransitionedInStart()
        {
            if (transitionIn != null)
            {
                transitionIn.Invoke();
            }
        }

        public virtual void OnTransitionedOutStart()
        {
            if (transitionOut != null)
            {
                transitionOut.Invoke();
            }
        }

        public virtual void OnTransitionedInEnd()
        {
            if (transitionIn != null)
            {
                transitionIn.Invoke();
            }
        }

        public virtual void OnTransitionedOutEnd()
        {
            if (transitionOut != null)
            {
                transitionOut.Invoke();
            }
        }

        void DelegateTransitionIn()
        {

        }

        void DelegateTransitionOut()
        {

        }
    }
}


