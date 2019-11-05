using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeState : MelodyState
{

    Vector3 Dodge = Vector3.left;

    public DodgeState(MelodyController controller) : base(controller)
    {
        Debug.Log("Entering DodgeState");
    }

    protected override void Enter()
    {
        Debug.Log("DODGE");
        melodyController.MAnimator.SetTrigger("Dodge");
        melodyController.MRigidBody.useGravity = false;
        Dodge = new Vector3(1 * Mathf.Sin(Mathf.Deg2Rad * melodyController.transform.eulerAngles.y), 0, Mathf.Cos(Mathf.Deg2Rad * melodyController.transform.eulerAngles.y));
        Dodge *= 4;
        Debug.Log(Dodge);
        Debug.Log(melodyController.transform.eulerAngles.y);
    }

    public override void OnUpdate(float time)
    {
        base.OnUpdate(time);

        melodyController.MRigidBody.velocity = Dodge;
        if (melodyController.MAnimator.IsInTransition(0) && melodyController.MAnimator.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
        {
            melodyController.MRigidBody.useGravity = true;
            melodyController.MRigidBody.velocity = Vector3.zero;
            melodyController.MAnimator.ResetTrigger("Dodge");
            AbleToExit = true;
        }
    }

    public override MelodyState NextState()
    {
        return new IdleState(melodyController);
    }
}
