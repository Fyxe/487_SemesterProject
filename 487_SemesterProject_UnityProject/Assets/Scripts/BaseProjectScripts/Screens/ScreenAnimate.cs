using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ScreenAnimate : Screen
    {

        Animator anim;
        public bool waitingOnTransition = false;

        int m_transitionTypeIn = 0;
        public int transitionTypeIn
        {
            get
            {
                return m_transitionTypeIn;
            }
            set
            {
                m_transitionTypeIn = value;
                if (anim != null)
                {
                    anim.SetInteger("int_transitionTypeIn", transitionTypeIn);
                }
            }
        }
        int m_transitionTypeOut = 0;
        public int transitionTypeOut
        {
            get
            {
                return m_transitionTypeOut;
            }
            set
            {
                m_transitionTypeOut = value;
                if (anim != null)
                {
                    anim.SetInteger("int_transitionTypeOut", transitionTypeOut);
                }
            }
        }

        void Awake()
        {
            anim = GetComponent<Animator>();            
            if (anim == null)
            {
                Debug.LogError("No animator was found attached to this ScreenAnimate gameobject " + name + " in Awake. No transitions will be able to occur. Consider adding a default fallback to this script.");
            }
        }

        void Start()
        {
            if (anim != null)
            {
                anim.SetInteger("int_transitionTypeIn", transitionTypeIn);
                anim.SetInteger("int_transitionTypeOut", transitionTypeOut);
                FinishedTransitionIn[] transitionsIn = anim.GetBehaviours<FinishedTransitionIn>();
                FinishedTransitionOut[] transitionsOut = anim.GetBehaviours<FinishedTransitionOut>();
                foreach (FinishedTransitionIn i in transitionsIn)
                {
                    i.Setup(this);
                }
                foreach (FinishedTransitionOut i in transitionsOut)
                {
                    i.Setup(this);
                }
            }
            else
            {
                Debug.LogError("No animator was found attached to this ScreenAnimate gameobject " + name + " in Start. No transitions will be able to occur. Consider adding a default fallback to this script.");
            }
        }

        public override IEnumerator TransitionIn()
        {
            if (anim == null)
            {
                Debug.LogError("No animator was found attached to this ScreenAnimate gameobject " + name + " in TransitionIn. No transitions will be able to occur. Consider adding a default fallback to this script.");
                yield break;
            }
            anim.SetTrigger("trigger_transitionIn");
            anim.SetBool("bool_isDisplayed", true);

            waitingOnTransition = true;
            while (waitingOnTransition)
            {
                yield return null;
            }
        }

        public override IEnumerator TransitionOut()
        {
            if (anim == null)
            {
                Debug.LogError("No animator was found attached to this ScreenAnimate gameobject " + name + " in TransitionOut. No transitions will be able to occur. Consider adding a default fallback to this script.");
                yield break;
            }
            anim.SetTrigger("trigger_transitionOut");
            anim.SetBool("bool_isDisplayed", false);

            waitingOnTransition = true;
            while (waitingOnTransition)
            {
                yield return null;
            }
        }

        public override void OnTransitionedIn()
        {
            base.OnTransitionedIn();
        }

        public override void OnTransitionedOut()
        {
            base.OnTransitionedOut();
        }
    }

}
