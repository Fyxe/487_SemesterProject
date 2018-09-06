using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{

    public class UITab : MonoBehaviour
    {
        TabContainer container;

        public GameObject attachedScreen;

        void Awake()
        {
            container = GetComponentInParent<TabContainer>();
            if (container == null)
            {
                Debug.LogError("There is no tab container for this tab.");
            }
        }

        public void OnTabClick()
        {
            if (container != null)
            {
                container.CloseAllButThisScreen(attachedScreen);
            }
            else
            {
                Debug.LogError("There is no tab container for this tab.");
            }
            
        }

    }

}
