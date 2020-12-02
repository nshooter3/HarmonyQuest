namespace Melody.States
{
    using UnityEngine;

    public class DashState : MelodyState
    {
        protected Vector3 dodge;
        protected float timer = 0.0f, maxTimer;
        private float dodgeMultiplier;

        private Vector3 normalizedVelocity;
        private float velocityMagnitude;

        bool startedRampJump = false;

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
            maxTimer = melodyController.config.DashTime;
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);

            timer += time;
            if (timer >= maxTimer)
            {
                ableToExit = true;
            }

            //Keep our y velocity if Melody has jumped off a ramp, allowing her to get some air.
            if (melodyController.melodyRamp.isRampDash)
            {
                //Adjustments to the dash that fire on the first frame of using a ramp.
                if (!startedRampJump)
                {
                    //Restart our dash timer to ensure that Melody gets the proper distance off of ramp jumps.
                    timer = 0f;
                    //Apply the dash duration multplier from the ramp, as some jumps will need to last longer.
                    maxTimer = melodyController.config.DashTime * melodyController.melodyRamp.dashDurationMultiplier;

                    //Face Melody in the direction of the ramp she jumped.
                    dodge.x = melodyController.melodyRamp.rampDirection.x;
                    dodge.y = 0f;
                    dodge.z = melodyController.melodyRamp.rampDirection.z;

                    //Recalculate dodge with our new direction, and apply the speed multiplier from the ramp.
                    dodge = dodge.normalized * dodgeMultiplier * melodyController.melodyRamp.dashSpeedMultiplier;
                    melodyController.melodyPhysics.InstantFaceDirection(dodge);

                    startedRampJump = true;
                }

                dodge.y = melodyController.GetRigidbodyVelocity().y;
            }

            //Restrict the Y axis range of Melody's dash once she leaves the ground.
            if (melodyController.melodyCollision.IsInAir())
            {
                normalizedVelocity = melodyController.GetRigidbodyVelocity().normalized;
                velocityMagnitude = melodyController.GetRigidbodyVelocity().magnitude;
                normalizedVelocity.y = Mathf.Clamp(normalizedVelocity.y, melodyController.config.dashYRadianAirLowerRange, melodyController.config.dashYRadianAirUpperRange);
                dodge.y = Mathf.Max(normalizedVelocity.y * velocityMagnitude, 0f);
            }
            melodyController.melodyPhysics.dashDirection = dodge;
        }

        public override void OnFixedUpdate()
        {
            melodyController.melodyPhysics.ApplyDashVelocity(dodge);

            if (melodyController.melodyRamp.isRampDash)
            {
                melodyController.melodyPhysics.ApplyRampGravity(melodyController.config.RampDashGravity);
            }
            else
            {
                melodyController.melodyPhysics.ApplyDashGravity(melodyController.config.GroundedDashGravity);
                melodyController.melodyPhysics.SnapToGround();
            }

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
