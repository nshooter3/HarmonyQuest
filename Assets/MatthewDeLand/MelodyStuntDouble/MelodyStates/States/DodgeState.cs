using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeState : MelodyState
{
    public DodgeState(MelodyController controller) : base(controller)
    {
        Debug.Log("Entering DodgeState");
    }

    protected override void Enter()
    {
        Debug.Log("DODGE");
        melodyController.MAnimator.SetTrigger("Dodge");

    }

    public override void OnUpdate(float time)
    {
        base.OnUpdate(time);
        if (melodyController.MAnimator.IsInTransition(0) && melodyController.MAnimator.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
        {
            melodyController.MAnimator.ResetTrigger("Dodge");
            AbleToExit = true;
        }
    }

    public override MelodyState NextState()
    {
        return new IdleState(melodyController);
    }
}
