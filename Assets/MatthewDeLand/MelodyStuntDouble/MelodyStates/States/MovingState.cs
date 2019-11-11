using UnityEngine;

public class MovingState : MelodyState
{
    public MovingState(MelodyController controller) : base(controller) { }

    protected override void Enter(){}

    public override void OnUpdate(float time)
    {
        base.OnUpdate(time);

        //Check For Attack
        if (melodyController.input.AttackButtonDown())
        {
           
            ableToExit = true;
            nextState = new AttackState(melodyController);
        }
        else if (melodyController.input.ParryButtonDown())
        {
            ableToExit = true;
            nextState = new CounterState(melodyController);
        }
        else if (melodyController.input.DodgeButtonDown())
        {
            ableToExit = true;
            nextState = new DodgeState(melodyController);
        }
        else if (melodyController.input.GetHorizontalMovement() == 0 && melodyController.input.GetVerticalMovement() == 0)
        {
            ableToExit = true;
            nextState = new IdleState(melodyController);
        }

        melodyController.animator.SetFloat("Move", melodyController.Move.magnitude / 1);
        melodyController.Move *= melodyController.MaxSpeed;
        RotatePlayer(3);

        melodyController.Move = melodyController.Move * time * 100;
        melodyController.rigidBody.velocity = new Vector3(melodyController.Move.x, melodyController.rigidBody.velocity.y, melodyController.Move.z);

    }

    void RotatePlayer(float turnSpeedModifier)
    {
        //Rotate player to face movement direction
        if (melodyController.Move.magnitude > 0)
        {
            Vector3 targetPos = melodyController.transform.position + melodyController.Move;
            Vector3 targetDir = targetPos - melodyController.transform.position;

            float step = 5 * turnSpeedModifier * Time.deltaTime;

            Vector3 newDir = Vector3.RotateTowards(melodyController.transform.forward, targetDir, step, 0.0f);
            Debug.DrawRay(melodyController.transform.position, newDir, Color.red);

            // Move our position a step closer to the target.
            melodyController.transform.rotation = Quaternion.LookRotation(newDir);
        }
        //Failsafe to ensure that x and z are always zero.
        melodyController.transform.eulerAngles = new Vector3(0, melodyController.transform.eulerAngles.y, 0);
    }

    public override void OnExit()
    {
        melodyController.animator.SetFloat("Move", 0f);
        melodyController.rigidBody.velocity = Vector3.zero;
    }
}
