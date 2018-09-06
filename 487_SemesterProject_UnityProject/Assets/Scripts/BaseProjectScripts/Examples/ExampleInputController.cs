using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examples
{
    [RequireComponent(typeof(CharacterController))]
    public class ExampleInputController : MonoBehaviour
    {
        [Header("Input Controller Settings")]
        public bool isControlled = true;
        public float speedMove = 6f;
        public float speedJump = 8f;
        public float forceGravity = 20f;

        Vector3 directionMove = Vector3.zero;

        CharacterController controllerCurrent;

        void Awake()
        {
            controllerCurrent = GetComponent<CharacterController>();
        }

        void Update()
        {            
            if (!isControlled)
            {
                directionMove.y -= forceGravity * Time.deltaTime;
                controllerCurrent.Move(directionMove * Time.deltaTime);
                return;
            }

            if (controllerCurrent.isGrounded)
            {
                directionMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                directionMove = transform.TransformDirection(directionMove);
                directionMove *= speedMove;
                if (Input.GetButton("Jump"))
                {
                    directionMove.y = speedJump;
                }                    
            }

            directionMove.y -= forceGravity * Time.deltaTime;

            controllerCurrent.Move(directionMove * Time.deltaTime);
        }
    }
}


