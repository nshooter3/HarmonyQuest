namespace Melody.States
{
    using UnityEngine;

    public class DashOutroState : MelodyState
    {
        protected float timer = 0;

        public DashOutroState(MelodyController controller) : base(controller) { }

        protected override void Enter()
        {
            melodyController.melodyAnimator.ExitDash();

            nextState = new IdleState(melodyController);
            timer = 0;
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);

            timer += time;
            if (timer >= melodyController.config.DashOutroTime)
            {
                if (melodyController.melodyPhysics.velocity.magnitude >= 0.0f)
                {
                    nextState = new MovingState(melodyController);
                }
                else
                {
                    nextState = new IdleState(melodyController);
                }
                ableToExit = true;
            }


            melodyController.melodyPhysics.CalculateVelocity(melodyController.config.DashOutroMaxSpeed, melodyController.config.MaxAcceleration);
        }

        public override void OnFixedUpdate()
        {
            melodyController.melodyPhysics.ApplyVelocity(melodyController.config.DashOutroMaxSpeed, melodyController.config.DashOutroTurningSpeed);
            melodyController.melodyPhysics.ApplyGravity(melodyController.config.DashOutroGravity);
            base.OnFixedUpdate();
        }

        public override void OnExit(){}
    }
}
