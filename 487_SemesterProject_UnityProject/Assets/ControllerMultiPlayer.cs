using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputController3D))]
public class ControllerMultiPlayer : Damageable
{

    [Header("Settings")]   
    public bool invertAxis1Z = false;

    [Header("References")]
    public Color colorPlayer;
    public int indexJoystick = 0;
    public int pointsCurrent = 0;    
    float m_speedMoveCurrent = 3f;
    public float speedMoveCurrent
    {
        get
        {
            return m_speedMoveCurrent;
        }
        set
        {
            m_speedMoveCurrent = value;
            controller.speedMove = speedMoveCurrent;
        }
    }
    public int damageBaseCurrent = 0;
    public int countReviveCurrent = 1;

    InputController3D controller;
    Animator anim;

    void Awake()
    {
        controller = GetComponent<InputController3D>();      
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (invertAxis1Z)
        {
            controller.SetAxis(
                Input.GetAxis("P" + indexJoystick.ToString() + "_Axis0Horizontal"),
                Input.GetAxis("P" + indexJoystick.ToString() + "_Axis0Vertical"),
                Input.GetAxis("P" + indexJoystick.ToString() + "_Axis1Horizontal"),
                Input.GetAxis("P" + indexJoystick.ToString() + "_Axis1Vertical") * (-1f)
                );
        }
        else
        {        
            controller.SetAxis(
                Input.GetAxis("P" + indexJoystick.ToString() + "_Axis0Horizontal"),
                Input.GetAxis("P" + indexJoystick.ToString() + "_Axis0Vertical"),
                Input.GetAxis("P" + indexJoystick.ToString() + "_Axis1Horizontal"),
                Input.GetAxis("P" + indexJoystick.ToString() + "_Axis1Vertical")
                );
        }

        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 0"))
        {
            AttemptSwapWeapons();
        }
        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 1"))
        {
            AttemptUseAbility();
        }
        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 2"))
        {
            AttemptRevive();   
        }
        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 3"))
        {
            AttemptThrowWeapon();
        }
        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 4"))
        {
            AttemptInteract();
        }
        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 5"))
        {
            AttemptAttack();
        }
        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 6"))
        {
            AttemptDisplayMoreInformation();
        }
        if (Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 8") && Input.GetKeyDown("joystick " + indexJoystick.ToString() + " button 9"))
        {
            AttemptThrowPoints();
        }
    }

    public void Setup(PlayerAttibutes newAttribute)
    {
        Debug.Log(newAttribute.indexJoystick);
        colorPlayer = newAttribute.colorPlayer;
        indexJoystick = newAttribute.indexJoystick;
        pointsCurrent = newAttribute.pointsCurrent;
        hpCurrent = newAttribute.hpCurrent;
        hpMax = newAttribute.hpMax;
        speedMoveCurrent = newAttribute.speedMoveCurrent;
        damageBaseCurrent = newAttribute.damageBaseCurrent;
        countReviveCurrent = newAttribute.countReviveCurrent;

        foreach (var i in GetComponentsInChildren<Renderer>())
        {
            i.material.color = colorPlayer;
        }        
    }

    public void AttemptInteract()
    {
        Debug.Log("P" + indexJoystick.ToString() + " tried to interact.");
    }

    public void AttemptRevive()
    {
        Debug.Log("P" + indexJoystick.ToString() + " tried to revive.");
    }

    public void AttemptAttack()
    {
        Debug.Log("P" + indexJoystick.ToString() + " tried to attack.");
    }

    public void AttemptSwapWeapons()
    {
        Debug.Log("P" + indexJoystick.ToString() + " tried to swap weapons.");
    }

    public void AttemptThrowWeapon()
    {
        Debug.Log("P" + indexJoystick.ToString() + " tried to throw their weapon.");
    }

    public void AttemptUseAbility()
    {
        Debug.Log("P" + indexJoystick.ToString() + " tried to use an ability.");
    }

    public void AttemptThrowPoints()
    {
        Debug.Log("P" + indexJoystick.ToString() + " tried to throw points.");
    }

    public void AttemptDisplayMoreInformation()
    {
        Debug.Log("P" + indexJoystick.ToString() + " tried to display more HUD information.");
    }

    void SetInvulnerable()
    {

    }

}
