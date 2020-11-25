namespace Melody.States
{
    using UnityEngine;

    public class DashIntroState : MelodyState
    {
        protected Vector3 dodge;
        protected float timer;
        private float dodgeMultiplier;

        public DashIntroState(MelodyController controller) : base(controller) { stateName = "DashIntroState"; }

        protected override void Enter()
        {
            dodgeMultiplier = (melodyController.config.DashLength / melodyController.config.DashTime);

            melodyController.melodyAnimator.EnterDash();

            //If there's no controller input, dodge in the direction of the player's forward
            if (melodyController.move == Vector3.zero)
            {
                dodge = melodyController.transform.forward * dodgeMultiplier;
            }
            else
            {
                //Use the controller input rather than the player velocity to determine dash direction.
                //This feels a lot more responsive and prevents edge cases like dashing backwards after sliding down a hill.
                dodge = melodyController.move.normalized * dodgeMultiplier;
                melodyController.melodyPhysics.InstantFaceDirection(dodge);
            }

            nextState = new DashState(melodyController, dodge, dodgeMultiplier);
            melodyController.melodyPhysics.ApplyStationaryVelocity();

            timer = 0;
            melodyController.melodyHealth.isDashing = true;
            melodyController.melodySound.Dash();
            melodyController.melodyHealth.CheckForLateDodges();
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);

            timer += time;
            
            if (timer >= melodyController.config.DashIntroTime)
            {
                ableToExit = true;
            }
        }

        public override void OnExit()
        {
            
        }
    }
}
