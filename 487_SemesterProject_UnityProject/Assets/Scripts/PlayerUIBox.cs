using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIBox : MonoBehaviour 
{
	public enum BoxSetting { alive, dead, empty }

	public BoxSetting setting = BoxSetting.empty;

	[Header("References")]
	public Image imageHealthBar;
	public Text textPoints;
	public Text textSpeedMove;
	public Text textDamageBase;
	public Text textReviveCount;
	public Animator anim;
	public GameObject objectAlive;
	public GameObject objectDead;
	public GameObject objectEmpty;

	void Awake()
	{
		anim = GetComponentInChildren<Animator>();
	}

	public void Set(BoxSetting newSetting)
	{
		objectAlive.SetActive(false);
		objectDead.SetActive(false);
		objectEmpty.SetActive(false);
		switch(newSetting)	
		{
			case BoxSetting.alive:
				objectAlive.SetActive(true);
				break;	
			case BoxSetting.dead:			
				objectDead.SetActive(true);
				break;	
			case BoxSetting.empty:				
				objectEmpty.SetActive(true);
				break;	
		}
	}

}
