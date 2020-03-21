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
            dodgeMultiplier = (melodyController.config.DashLength / melodyController.config.DashTime);
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
            //Restrict the Y axis range of Melody's dash once she leaves the ground.
            if (melodyController.melodyCollision.IsInAir())
            {
                dodge.y = Mathf.Clamp(dodge.y / dodgeMultiplier, melodyController.config.dashYRadianAirLowerRange, melodyController.config.dashYRadianAirUpperRange) * dodgeMultiplier;
            }
            else
            {
                dodge.y = Mathf.Clamp(dodge.y / dodgeMultiplier, melodyController.config.dashYRadianGroundedLowerRange, melodyController.config.dashYRadianGroundedUpperRange) * dodgeMultiplier;
            }
            //Debug.Log("DODGE RADIANS: " + dodge / dodgeMultiplier);
        }

        public override void OnFixedUpdate()
        {
            melodyController.melodyPhysics.ApplyDashVelocity(dodge);
            melodyController.melodyPhysics.SnapToGround();
            base.OnFixedUpdate();
        }

        public override void OnExit()
        {
            melodyController.melodyPhysics.CapSpeed(melodyController.config.MaxSpeed);
        }
    }
}
