using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgressor : MonoBehaviour 
{

    [Header("Settings")]
    public bool isEnd = true;
	public List<ControllerMultiPlayer> playersEntered = new List<ControllerMultiPlayer>();

    [Header("References")]
    public GameObject toDisable;

    bool fired = false;

	ControllerMultiPlayer cachedPlayer;

    void Awake()
    {
        if (toDisable != null)
        {
            toDisable.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider col)
	{
		if (fired || col.isTrigger || ((cachedPlayer = col.GetComponentInParent<ControllerMultiPlayer>()) == null) || playersEntered.Contains(cachedPlayer))
		{            
            return;
		}
		else
		{
			playersEntered.Add(cachedPlayer);
			if (playersEntered.Count == PlayerManager.instance.playersInGame)
			{
                if (toDisable != null)
                {
                    toDisable.SetActive(false);
                }

                fired = true;
                if (isEnd)
                {
                    LevelManager.instance.EndLevel(true);
                }
                else
                {
                    LevelManager.instance.StartLevel();
                }
			}
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (fired || col.isTrigger || ((cachedPlayer = col.GetComponentInParent<ControllerMultiPlayer>()) == null) || !playersEntered.Contains(cachedPlayer))
		{
			return;
		}
		else
		{
			playersEntered.Remove(cachedPlayer);
		}
	}
}
