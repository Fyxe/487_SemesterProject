﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIBox : MonoBehaviour 
{
	public enum BoxSetting { alive, dead, incapacitated, empty }

	public BoxSetting setting = BoxSetting.empty;

	[Header("References")]
    public Image imageReviveTimer;
    public Image imageReviveCount;
    public Text textPoints;
	public Text textSpeedMove;
	public Text textDamageBase;
	public Text textReviveCount;
	public Animator anim;
	public GameObject objectAlive;
	public GameObject objectDead;
    public GameObject objectIncapacitated;
    public GameObject objectEmpty;
    public Transform parentHealth;

    [Header("Prefabs")]
    public GameObject prefabHealth;

	void Awake()
	{
		anim = GetComponentInChildren<Animator>();
	}

    // TODO sync weapon names and bullets in clip

    // TODO animation for aquiring points / spending points

	public void Set(BoxSetting newSetting)
	{
        setting = newSetting;
		objectAlive.SetActive(false);
        objectDead.SetActive(false);
        objectIncapacitated.SetActive(false);
        objectEmpty.SetActive(false);
		switch(newSetting)	
		{
			case BoxSetting.alive:
				objectAlive.SetActive(true);
				break;	
			case BoxSetting.dead:			
                if (anim.GetBool("IsUp"))
                {
                    ToggleSize(); 
                }
				objectDead.SetActive(true);
				break;
            case BoxSetting.incapacitated:
                if (anim.GetBool("IsUp"))
                {
                    ToggleSize();
                }
                objectIncapacitated.SetActive(true);
                break;
			case BoxSetting.empty:
                if (anim.GetBool("IsUp"))
                {
                    ToggleSize();
                }
                objectEmpty.SetActive(true);
				break;	
		}
	}

    public void ToggleSize()
    {        
        anim.SetTrigger("ChangeDisplay");
        anim.SetBool("IsUp", !anim.GetBool("IsUp"));
    }

    public void SetHealth(int amountCurrent, int amountMax, Color colorToUse)
    {
        parentHealth.DestroyChildren();
        for (int i = 1; i <= amountMax; i++)
        {
            GameObject spawnedHealthObject = Instantiate(prefabHealth,parentHealth);
            Image spawnedHealthImage = spawnedHealthObject.GetComponent<Image>();
            spawnedHealthImage.color = colorToUse;
            if (i > amountCurrent)
            {
                spawnedHealthImage.sprite = PlayerManager.instance.spriteHealthEmpty;
            }
            else
            {
                spawnedHealthImage.sprite = PlayerManager.instance.spriteHealthFull;
            }
        }
    }
}
