using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIBox : MonoBehaviour 
{
	[Header("References")]
	public Image imageHealthBar;
	public Text textPoints;
	public Text textSpeedMove;
	public Text textDamageBase;
	public Text textReviveCount;
	public Animator anim;

	void Awake()
	{
		anim = GetComponentInChildren<Animator>();
	}

}
