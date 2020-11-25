namespace Melody.States
{
    using UnityEngine;

    public class DashState : MelodyState
    {
        protected Vector3 dodge;
        protected float timer = 0.0f;
        private float dodgeMultiplier;

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

            //Apply the y value from our velocity so that we maintain our upwards/downwards momentum when dashing.
            dodge.y = melodyController.GetVelocity().y ;

            //Debug.Log("DODGE: " + dodge);
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
