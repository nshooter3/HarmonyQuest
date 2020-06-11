namespace Melody.States
{
    using UnityEngine;

    public class DashIntroState : MelodyState
    {

        protected Vector3 dodge;
        protected float timer;

        public DashIntroState(MelodyController controller) : base(controller) { }

        protected override void Enter()
        {
            melodyController.melodyAnimator.EnterDash();

            //If there's no controller input, dodge in the direction of the player's forward
            if (melodyController.move == Vector3.zero)
            {
                dodge = melodyController.transform.forward * (melodyController.config.DashLength / melodyController.config.DashTime);
            }
            else
            {
                //Use the controller input rather than the player velocity to determine dash direction.
                //This feels a lot more responsive and prevents edge cases like dashing backwards after sliding down a hill.
                dodge = melodyController.move;
                //Apply the normalized y value from our velocity so that we maintain our upwards/downwards momentum when dashing. This allows Melody to get extra height off of ramps.
                dodge.y = melodyController.rigidBody.velocity.normalized.y;
                dodge = dodge.normalized * (melodyController.config.DashLength / melodyController.config.DashTime);
            }

            nextState = new DashState(melodyController, dodge);
            melodyController.rigidBody.velocity = Vector3.zero;

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
