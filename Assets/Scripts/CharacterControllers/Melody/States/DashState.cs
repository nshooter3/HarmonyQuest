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
            stateName = "DashState";
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

            //Apply the normalized y value from our velocity so that we maintain our upwards/downwards momentum when dashing.
            //This allows Melody to get extra height off of ramps.
            dodge.y = melodyController.rigidBody.velocity.normalized.y * dodgeMultiplier;

            //Restrict the Y axis range of Melody's dash once she leaves the ground.
            if (melodyController.melodyCollision.IsInAir())
            {
                dodge.y = Mathf.Clamp(dodge.y / dodgeMultiplier, melodyController.config.dashYRadianAirLowerRange, melodyController.config.dashYRadianAirUpperRange) * dodgeMultiplier;
            }
            //Debug.Log("DODGE RADIANS: " + dodge / dodgeMultiplier);
        }

        public override void OnFixedUpdate()
        {
            melodyController.melodyPhysics.ApplyDashVelocity(dodge);
            melodyController.melodyPhysics.ApplyDashGravity(melodyController.config.GroundedDashGravity);
            //Cap speed after applying gravity when grounded to prevent Melody from moving too quickly downhill.
            if (melodyController.melodyCollision.IsGrounded() == true)
            {
                melodyController.melodyPhysics.CapSpeed(dodgeMultiplier);
            }
            melodyController.melodyPhysics.SnapToGround();

            //Debug.Log("Dash State Velocity Magnitude: " + melodyController.melodyPhysics.velocity.magnitude);

            base.OnFixedUpdate();
        }

        public override void OnExit()
        {
            melodyController.melodyPhysics.OverrideVelocity(dodge);
            melodyController.melodyPhysics.CapSpeed(melodyController.config.MaxSpeed);
            melodyController.melodyPhysics.ApplyVelocity(melodyController.config.DashOutroMaxSpeed, melodyController.config.DashOutroTurningSpeed);
        }
    }
}
