namespace Melody.States
{
    using UnityEngine;

    public class DashOutroState : MelodyState
    {
        protected float timer = 0;

        public DashOutroState(MelodyController controller) : base(controller) { }

        protected override void Enter()
        {
            //melodyController.animator.SetTrigger("DashOutro");
            melodyController.melodyRenderer.enabled = true;
            melodyController.scarfRenderer.enabled = false;

            nextState = new IdleState(melodyController);
            timer = 0;
            melodyController.rigidBody.velocity = Vector3.zero;
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);

            timer += time;
            if (timer >= melodyController.config.DashOutroTime)
            {
                nextState = new IdleState(melodyController);
                ableToExit = true;
            }

            melodyController.melodyPhysics.CalculateVelocity(melodyController.config.DashOutroMaxSpeed, melodyController.config.MaxAcceleration);
        }

        public override void OnFixedUpdate()
        {
            melodyController.melodyPhysics.ApplyVelocity(melodyController.config.DashOutroMaxSpeed, melodyController.config.DashOutroTurningSpeed);
            melodyController.melodyPhysics.ApplyGravity(melodyController.config.DashOutroGravity);
        }

        public override void OnExit()
        {            
            melodyController.animator.ResetTrigger("Dash");
            melodyController.animator.ResetTrigger("DashOutro");
        }
    }
}
