using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDash : Ability
{

    [Header("Dash Settings")]
    public float speed = 8f;
    Vector3 direction;
    public AudioClip sound;

    public override bool AttemptAttack()
    {
        if (base.AttemptAttack())
        {
            Vector3 direction = controllerCurrent.attachedPlayer.controllerInput.GetAxis(0);
            return direction != Vector3.zero;
        }
        else
        {
            return false;
        }
    }

    protected override bool Attack()
    {
        direction = controllerCurrent.attachedPlayer.controllerInput.GetAxis(0);
        AudioManager.instance.PlayClipLocalSpace(sound);
        if (direction == Vector3.zero)
        {
            return false;
        }
        return base.Attack();
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        controllerCurrent.attachedPlayer.isDashing = true;
        controllerCurrent.attachedPlayer.controllerInput.isControlled = false;
        controllerCurrent.attachedPlayer.rb.velocity = Vector3.zero;
        controllerCurrent.attachedPlayer.rb.AddForce(direction.normalized * speed, ForceMode.Impulse);
    }

    public override void OnAbilityEnd()
    {
        controllerCurrent.attachedPlayer.rb.velocity = Vector3.zero;
        controllerCurrent.attachedPlayer.controllerInput.isControlled = true;
        controllerCurrent.attachedPlayer.isDashing = false;
        base.OnAbilityEnd();
    }

}
