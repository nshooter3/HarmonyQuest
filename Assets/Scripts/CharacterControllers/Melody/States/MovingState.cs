namespace Melody.States
{
    using UnityEngine;

    public class MovingState : MelodyState
    {
        public MovingState(MelodyController controller) : base(controller) { }

        protected override void Enter() { }

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
                nextState = new DashIntroState(melodyController);
            }
            else if (melodyController.move.magnitude == 0.0f && melodyController.melodyPhysics.velocity.magnitude == 0.0f)
            {
                ableToExit = true;
                nextState = new IdleState(melodyController);
            }

            melodyController.melodyPhysics.CalculateVelocity();
        }

        public override void OnFixedUpdate()
        {
            melodyController.melodyPhysics.ApplyVelocity();
            melodyController.melodyPhysics.ApplyGravity();
        }

        public override void OnExit()
        {
            melodyController.animator.SetFloat("Move", 0.0f);
        }
    }
}