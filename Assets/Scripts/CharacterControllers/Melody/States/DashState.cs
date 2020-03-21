namespace Melody.States
{
    using UnityEngine;

    public class DashState : MelodyState
    {

        protected Vector3 dodge;
        protected float timer = 0.0f;
        private float dodgeMultiplier;

        public DashState(MelodyController controller, Vector3 dodge) : base(controller)
        {
            this.dodge = dodge;
        }

        protected override void Enter()
        {
            nextState = new DashOutroState(melodyController);
            timer = 0.0f;
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);

            timer += time;
            if (timer >= melodyController.config.DashTime)
            {
                ableToExit = true;
            }
            if (melodyController.melodyCollision.IsInAir())
            {
                //Restrict the Y axis range of Melody's dash once she leaves the ground.
                dodgeMultiplier = (melodyController.config.DashLength / melodyController.config.DashTime);
                dodge.y = Mathf.Clamp(dodge.y/dodgeMultiplier, melodyController.config.dashYRadianLowerRange, melodyController.config.dashYRadianUpperRange) * dodgeMultiplier;
            }
        }

        public override void OnFixedUpdate()
        {
            melodyController.melodyPhysics.ApplyDashVelocity(dodge);
            //melodyController.rigidBody.velocity = dodge;
            melodyController.melodyPhysics.SnapToGround();
            base.OnFixedUpdate();
        }

        public override void OnExit()
        {
            melodyController.melodyPhysics.CapSpeed(melodyController.config.MaxSpeed);
        }
    }
}
