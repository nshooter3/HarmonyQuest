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
            //melodyController.animator.SetTrigger("Dash");
            melodyController.melodyRenderer.enabled = false;
            melodyController.scarfRenderer.enabled = true;
            
            //If there's no controller input, dodge backwards.
            if (melodyController.input.GetHorizontalMovement() == 0 && melodyController.input.GetVerticalMovement() == 0)
            {
                dodge = -melodyController.transform.forward * (melodyController.config.DashLength / melodyController.config.DashTime);
            }
            else
            {
                dodge = melodyController.rigidBody.velocity.normalized * (melodyController.config.DashLength / melodyController.config.DashTime);
            }

            nextState = new DashState(melodyController, dodge);
            melodyController.rigidBody.velocity = Vector3.zero;

            timer = 0;
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
