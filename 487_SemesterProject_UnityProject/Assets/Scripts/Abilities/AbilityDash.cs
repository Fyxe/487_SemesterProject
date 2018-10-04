using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDash : Ability
{

    [Header("Dash Settings")]
    public float speed = 8f;

    Coroutine coroutineActivated;


    public override bool AttemptAttack()
    {
        if (base.AttemptAttack())
        {
            return rb.velocity != Vector3.zero;
        }
        else
        {
            return false;
        }
    }

    protected override bool Attack()
    {
        base.Attack();
        if (controllerCurrent.controllerType == ControllerType.player)
        {            
            if (coroutineActivated != null)
            {
                StopCoroutine(coroutineActivated);
            }
            coroutineActivated = StartCoroutine(Activated(controllerCurrent.attachedPlayer.controllerInput.GetAxis(0)));
        }
        else
        {

        }
        return true;
    }

    IEnumerator Activated(Vector3 direction)    // TODO work on paused
    {
        isInUse = true;        
        controllerCurrent.attachedPlayer.isDashing = true;
        controllerCurrent.attachedPlayer.controllerInput.isControlled = false;
        controllerCurrent.attachedPlayer.rb.velocity = Vector3.zero;        
        controllerCurrent.attachedPlayer.rb.AddForce(direction.normalized * speed, ForceMode.Impulse);
        yield return new WaitForSeconds(durationAttack);
        controllerCurrent.attachedPlayer.rb.velocity = Vector3.zero;
        controllerCurrent.attachedPlayer.controllerInput.isControlled = true;
        controllerCurrent.attachedPlayer.isDashing = false;
        isInUse = false;
    }

}
