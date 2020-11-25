namespace Melody.States
{
    using UnityEngine;

    public class DashState : MelodyState
    {
        protected Vector3 dodge;
        protected float timer = 0.0f;
        private float dodgeMultiplier;

        private Vector3 normalizedVelocity;
        private float velocityMagnitude;

        public DashState(MelodyController controller, Vector3 dodge, float dodgeMultiplier) : base(controller)
        {
            this.dodge = dodge;
            this.dodgeMultiplier = dodgeMultiplier;
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

            //TODO: Apply this if Melody is going up a ramp.
            //dodge.y = melodyController.GetRigidbodyVelocity().y;

            //Restrict the Y axis range of Melody's dash once she leaves the ground.
            if (melodyController.melodyCollision.IsInAir())
            {
                normalizedVelocity = melodyController.GetRigidbodyVelocity().normalized;
                velocityMagnitude = melodyController.GetRigidbodyVelocity().magnitude;
                normalizedVelocity.y = Mathf.Clamp(normalizedVelocity.y, melodyController.config.dashYRadianAirLowerRange, melodyController.config.dashYRadianAirUpperRange);
                dodge.y = Mathf.Max(normalizedVelocity.y * velocityMagnitude, 0f);
            }
        }

        public override void OnFixedUpdate()
        {
            melodyController.melodyPhysics.ApplyDashVelocity(dodge);
            melodyController.melodyPhysics.ApplyDashGravity(melodyController.config.GroundedDashGravity);
            melodyController.melodyPhysics.SnapToGround();

            base.OnFixedUpdate();
        }

        public override void OnExit()
        {
            //Only carry momentum into the DashOutroState if we don't run into a wall or a steep hill.
            if (melodyController.GetVelocity().magnitude > 0f && !melodyController.melodyCollision.IsSliding())
            {
                melodyController.melodyPhysics.OverrideVelocity(dodge);
            }
            else
            {
                melodyController.melodyPhysics.OverrideVelocity(Vector3.zero);
            }

            melodyController.melodyPhysics.CapSpeed(melodyController.config.MaxSpeed);
            melodyController.melodyPhysics.ApplyVelocity(melodyController.config.DashOutroMaxSpeed, melodyController.config.DashOutroTurningSpeed);

            if (melodyController.HasLockonTarget())
            {
                melodyController.melodyPhysics.InstantFaceDirection(melodyController.GetLockonTarget().aiGameObject.transform.position - melodyController.transform.position);
            }
            else
            {
                melodyController.melodyPhysics.InstantFaceDirection(dodge);
            }
        }
    }
}
