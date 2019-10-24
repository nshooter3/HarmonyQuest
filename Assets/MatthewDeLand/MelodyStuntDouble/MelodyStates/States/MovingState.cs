using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : MelodyState
{
    private MelodyState mNextState;

    public MovingState(MelodyController controller) : base(controller) { }

    protected override void Enter()
    {
        Debug.Log(" Entering MovingState!");
    }

    public override void OnUpdate(float time)
    {
        base.OnUpdate(time);

        //Check For Attack
        if (melodyController.MInput.AttackButtonDown())
        {
           
            AbleToExit = true;
            mNextState = new AttackState(melodyController);
        }
        else if (melodyController.MInput.ParryButtonDown())
        {
            AbleToExit = true;
            mNextState = new CounterState(melodyController);
        }
        else if (melodyController.MInput.DodgeButtonDown())
        {
            AbleToExit = true;
            mNextState = new DodgeState(melodyController);
        }
        else if (melodyController.MInput.GetHorizontalMovement() == 0 && melodyController.MInput.GetVerticalMovement() == 0)
        {
            AbleToExit = true;
            melodyController.MAnimator.SetFloat("Move", 0f);
            mNextState = new IdleState(melodyController);
        }

        melodyController.MAnimator.SetFloat("Move", melodyController.Move.magnitude / 1);
        melodyController.Move *= melodyController.MaxSpeed;
        RotatePlayer(3);

        melodyController.MCharacterController.Move(melodyController.Move * time);

    }

    public override MelodyState NextState()
    {
        return mNextState;
    }

    void RotatePlayer(float turnSpeedModifier)
    {
        //Rotate player to face movement direction
        if (melodyController.Move.magnitude > 0)
        {
            Vector3 targetPos = melodyController.transform.position + melodyController.Move;
            Vector3 targetDir = targetPos - melodyController.transform.position;
            //If locked on, ignore movement direction and always attempt to face enemy
            /*if (isLockedOn)
            {
                targetDir = lockOnTarget.transform.position - transform.position;
            }*/

            // The step size is equal to speed times frame time.
            float step = 5 * turnSpeedModifier * Time.deltaTime;

            Vector3 newDir = Vector3.RotateTowards(melodyController.transform.forward, targetDir, step, 0.0f);
            Debug.DrawRay(melodyController.transform.position, newDir, Color.red);

            // Move our position a step closer to the target.
            melodyController.transform.rotation = Quaternion.LookRotation(newDir);
        }
        //Failsafe to ensure that x and z are always zero.
        melodyController.transform.eulerAngles = new Vector3(0, melodyController.transform.eulerAngles.y, 0);
    }
}
