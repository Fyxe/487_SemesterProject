using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ScreenEnabler : ScreenBase
    {
        [Header("Screen Enabler Settings")]
        public bool beginningActiveState = false;

        void Awake()
        {
            this.gameObject.SetActive(beginningActiveState);
        }

        public override IEnumerator TransitionIn()
        {
            base.TransitionIn();
            this.gameObject.SetActive(true);
            yield break;
        }

        public override IEnumerator TransitionOut()
        {
            base.TransitionOut();
            this.gameObject.SetActive(false);
            yield break;
        }
    }
}
