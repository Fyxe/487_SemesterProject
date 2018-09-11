using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class InputController3D : MonoBehaviour
{
    [Header("Input Controller Settings")]
    public bool isControlled = true;
    public bool useFirstPerson = false;
    public float lookLerpSpeed = 0.2f;
    [Space]
    public bool canMove = true;
    [Space]
    public bool canLookOrAim = true;
    bool isAimingInDirectionOfMovement = false;
    public float delayResetAim = 1.5f;
    float nextResetAim = 0;
    [Space]
    public float speedMove = 6f;
    public float speedJump = 8f;
    public float forceGravity = 20f;
    [Space]
    public float speedRotationX = 5f;
    public float speedRotationZ = 4f;
    public float lookXAngleMinimum = -90f;
    public float lookXAngleMaximum = 90f;    

    [Header("Axis")]
    public float axis0X = 0f;
    public float axis0Z = 0f;
    [Space]
    public float axis1X = 0f;
    public float axis1Z = 0f;

    [Header("References")]
    public Transform parentCamera;

    Vector3 directionMove = Vector3.zero;

    CharacterController controllerCurrent;

    void Awake()
    {
        controllerCurrent = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (canMove)
        {
            Movement();
        }
        if (canLookOrAim)
        {
            if (useFirstPerson)
            {
                RotationCamera();
            }            
            else            
            {
                RotationController();
            }
        }        
    }

    public void SetAxis(float newMoveX, float newMoveZ, float newLookX, float newLookZ)
    {
        axis0X = newMoveX;
        axis0Z = newMoveZ;
        axis1X = newLookX;
        axis1Z = newLookZ;
    }

    void Movement()
    {
        if (!isControlled)
        {
            directionMove.y -= forceGravity * Time.deltaTime;
            controllerCurrent.Move(directionMove * Time.deltaTime);
            return;
        }

        if (controllerCurrent.isGrounded)
        {
            directionMove = new Vector3(axis0X, 0, axis0Z).normalized;
            if (useFirstPerson)
            {
                directionMove = transform.TransformDirection(directionMove);
            }            
            directionMove *= speedMove;
        }

        directionMove.y -= forceGravity * Time.deltaTime;

        controllerCurrent.Move(directionMove * Time.deltaTime);
    }

    void RotationCamera()   // FPS
    {        
        transform.rotation *= Quaternion.Euler(0f, axis1X * speedRotationX, 0f);        

        Quaternion targetRotation = parentCamera.rotation;
        targetRotation *= Quaternion.Euler(-1f * axis1Z * speedRotationZ, 0f, 0f);

        parentCamera.rotation = Quaternion.Slerp(parentCamera.rotation, targetRotation, 0.9f);

        parentCamera.localRotation = Quaternion.Slerp(parentCamera.localRotation, ClampRotationAroundXAxis(parentCamera.localRotation), 5f * Time.deltaTime);
    }

    void RotationController()
    {        
        if (Mathf.Approximately(axis1X + axis1Z, 0f))
        {
            if (Time.time > nextResetAim)
            {
                
                isAimingInDirectionOfMovement = true;
            }
        }
        else 
        {
            isAimingInDirectionOfMovement = false;
            nextResetAim = Time.time + delayResetAim;
        }

        if (isAimingInDirectionOfMovement)
        {
            if (!Mathf.Approximately(axis0X + axis0Z, 0f))
            {                
                transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(new Vector3(axis0X,0f,axis0Z).normalized),lookLerpSpeed); 
            }            
        }
        else
        {
            float angleY = Mathf.Atan2(axis1X, axis1Z) * Mathf.Rad2Deg;
            if (!Mathf.Approximately(angleY,0f))
            {
                transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(0f, angleY, 0f),lookLerpSpeed);
            }            
        }
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        // blatantly ripped from unity standard assets MouseLook.cs
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, lookXAngleMinimum, lookXAngleMaximum);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}



