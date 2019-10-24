using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : MelodyState
{
    public AttackState(MelodyController controller) : base(controller)
    {
        Debug.Log("Entering AttackState");
    }

    protected override void Enter()
    {
        melodyController.MAnimator.SetTrigger("Attack");
        
    }

    public override void OnUpdate(float time)
    {
        base.OnUpdate(time);
        if (melodyController.MAnimator.IsInTransition(0) && melodyController.MAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            melodyController.MAnimator.ResetTrigger("Attack");
            AbleToExit = true;
        }
    }

    public override MelodyState NextState()
    {        
        return new IdleState(melodyController);
    }
}
