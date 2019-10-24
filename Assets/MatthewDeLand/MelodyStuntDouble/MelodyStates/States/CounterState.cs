using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterState : MelodyState
{
    public CounterState(MelodyController controller) : base(controller)
    {
        Debug.Log("Entering CounterState");
    }

    protected override void Enter()
    {
        melodyController.MAnimator.SetTrigger("Counter");

    }

    public override void OnUpdate(float time)
    {
        base.OnUpdate(time);
        if (melodyController.MAnimator.IsInTransition(0) && melodyController.MAnimator.GetCurrentAnimatorStateInfo(0).IsName("Counter"))
        {
            melodyController.MAnimator.ResetTrigger("Counter");
            AbleToExit = true;
        }
    }

    public override MelodyState NextState()
    {
        return new IdleState(melodyController);
    }
}
