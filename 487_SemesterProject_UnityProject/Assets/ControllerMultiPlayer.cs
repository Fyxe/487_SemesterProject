using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputController3D))]
public class ControllerMultiPlayer : MonoBehaviour
{

    [Header("Settings")]
    public int playerIndex;   
    public bool invertAxis1Z = false;

    [Header("References")]
    public Color colorPlayer;
    public int indexPlayer = 0;
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
            input.speedMove = speedMoveCurrent;
        }
    }
    public int damageBaseCurrent = 0;
    public int countReviveCurrent = 1;

    InputController3D input;

    void Awake()
    {
        input = GetComponent<InputController3D>();      
        foreach (var i in GetComponentsInChildren<Renderer>())
        {
            i.material.color = colorPlayer;
        }
        GetComponentInChildren<Camera>().backgroundColor = colorPlayer;
    }

    void Update()
    {
        if (invertAxis1Z)
        {
            input.SetAxis(
                Input.GetAxis("P" + playerIndex.ToString() + "_Axis0Horizontal"),
                Input.GetAxis("P" + playerIndex.ToString() + "_Axis0Vertical"),
                Input.GetAxis("P" + playerIndex.ToString() + "_Axis1Horizontal"),
                Input.GetAxis("P" + playerIndex.ToString() + "_Axis1Vertical") * (-1f)
                );
        }
        else
        {        
            input.SetAxis(
                Input.GetAxis("P" + playerIndex.ToString() + "_Axis0Horizontal"),
                Input.GetAxis("P" + playerIndex.ToString() + "_Axis0Vertical"),
                Input.GetAxis("P" + playerIndex.ToString() + "_Axis1Horizontal"),
                Input.GetAxis("P" + playerIndex.ToString() + "_Axis1Vertical")
                );
        }

        if (Input.GetKeyDown("joystick " + playerIndex.ToString() + " button 0"))
        {
            AttemptSwapWeapons();
        }
        if (Input.GetKeyDown("joystick " + playerIndex.ToString() + " button 1"))
        {
            AttemptUseAbility();
        }
        if (Input.GetKeyDown("joystick " + playerIndex.ToString() + " button 2"))
        {
            AttemptRevive();   
        }
        if (Input.GetKeyDown("joystick " + playerIndex.ToString() + " button 3"))
        {
            AttemptThrowWeapon();
        }
        if (Input.GetKeyDown("joystick " + playerIndex.ToString() + " button 4"))
        {
            AttemptInteract();
        }
        if (Input.GetKeyDown("joystick " + playerIndex.ToString() + " button 5"))
        {
            AttemptAttack();
        }
    }

    public void Setup(PlayerAttibutes newAttribute)
    {
        colorPlayer = newAttribute.colorPlayer;
        indexPlayer = newAttribute.indexPlayer;
        pointsCurrent = newAttribute.pointsCurrent;
        speedMoveCurrent = newAttribute.speedMoveCurrent;
        damageBaseCurrent = newAttribute.damageBaseCurrent;
        countReviveCurrent = newAttribute.countReviveCurrent;        
    }

    public void AttemptInteract()
    {
        Debug.Log("P" +playerIndex.ToString() + " tried to interact.");
    }

    public void AttemptRevive()
    {
        Debug.Log("P" + playerIndex.ToString() + " tried to revive.");
    }

    public void AttemptAttack()
    {
        Debug.Log("P" + playerIndex.ToString() + " tried to attack.");
    }

    public void AttemptSwapWeapons()
    {
        Debug.Log("P" + playerIndex.ToString() + " tried to swap weapons.");
    }

    public void AttemptThrowWeapon()
    {
        Debug.Log("P" + playerIndex.ToString() + " tried to throw their weapon.");
    }

    public void AttemptUseAbility()
    {
        Debug.Log("P" + playerIndex.ToString() + " tried to use an ability.");
    }

}
