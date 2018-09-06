using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class TabContainer : MonoBehaviour
    {
        List<GameObject> screens = new List<GameObject>();
        List<UITab> allTabs = new List<UITab>();

        void Awake()
        {
            allTabs = GetComponentsInChildren<UITab>().ToList();
            foreach (var i in allTabs)
            {
                screens.Add(i.attachedScreen);
            }
            if (allTabs.Count > 0)
            {
                CloseAllButThisScreen(allTabs[0].attachedScreen);
            }
            else
            {
                Debug.LogWarning("No tabs were found for this tab container.");
            }
            
        }

        void CloseAllScreens()
        {
            foreach (var i in screens)
            {
                if (i != null)
                {
                    i.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("Attempted to close a null screen.");
                }
            }
        }

        public void CloseAllButThisScreen(GameObject newScreen)
        {
            CloseAllScreens();
            if (newScreen != null)
            {
                newScreen.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Attempted to open a null screen.");
            }
        }
    }

}
