using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputController3D))]
public class ControllerMultiPlayer : MonoBehaviour
{

    [Header("Settings")]
    public string playerPrefix = "P?";
    public bool setColor = true;
    public Color colorPlayer;
    public bool invertAxis1Z = false;

    //[Header("References")]
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
                Input.GetAxisRaw(playerPrefix + "_Axis0Horizontal"),
                Input.GetAxisRaw(playerPrefix + "_Axis0Vertical"),
                Input.GetAxisRaw(playerPrefix + "_Axis1Horizontal"),
                Input.GetAxisRaw(playerPrefix + "_Axis1Vertical") * (-1f)
                );
        }
        else
        {        
            input.SetAxis(
                Input.GetAxisRaw(playerPrefix + "_Axis0Horizontal"),
                Input.GetAxisRaw(playerPrefix + "_Axis0Vertical"),
                Input.GetAxisRaw(playerPrefix + "_Axis1Horizontal"),
                Input.GetAxisRaw(playerPrefix + "_Axis1Vertical")
                );
        }

        if (Input.GetButtonDown(playerPrefix + "_Jump"))
        {
            input.AttemptJump();         
        }        
    }

}
