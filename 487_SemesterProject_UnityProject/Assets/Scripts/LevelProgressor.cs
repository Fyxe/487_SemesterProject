using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgressor : MonoBehaviour 
{
	public List<ControllerMultiPlayer> playersEntered = new List<ControllerMultiPlayer>();

	ControllerMultiPlayer cachedPlayer;
	void OnTriggerEnter(Collider col)
	{
		if (col.isTrigger || ((cachedPlayer = col.GetComponentInParent<ControllerMultiPlayer>()) == null) || playersEntered.Contains(cachedPlayer))
		{
			return;
		}
		else
		{
			playersEntered.Add(cachedPlayer);
			if (playersEntered.Count == PlayerManager.instance.playersInGame)
			{
				LevelManager.instance.EndLevel(true);
			}
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.isTrigger || ((cachedPlayer = col.GetComponentInParent<ControllerMultiPlayer>()) == null) || !playersEntered.Contains(cachedPlayer))
		{
			return;
		}
		else
		{
			playersEntered.Remove(cachedPlayer);
		}
	}
}
