namespace Melody.States
{
    using System;
    using UnityEngine;

    public class DashIntroState : MelodyState
    {

        protected Vector3 dodge = Vector3.left;
        protected float timer;

        public DashIntroState(MelodyController controller) : base(controller)
        {
        }

        protected override void Enter()
        {
            melodyController.animator.SetTrigger("Dash");
            melodyController.rigidBody.useGravity = false;
            if (Math.Abs(melodyController.rigidBody.velocity.magnitude) > 0)
            {
                dodge = melodyController.rigidBody.velocity.normalized * (melodyController.config.DashLength / melodyController.config.DashTime);
            }
            else
            {
                dodge = new Vector3(1 * Mathf.Sin(Mathf.Deg2Rad * melodyController.transform.eulerAngles.y), 0, Mathf.Cos(Mathf.Deg2Rad * melodyController.transform.eulerAngles.y));
                dodge *= -(melodyController.config.DashLength / melodyController.config.DashTime);
            }
            nextState = new DashState(melodyController, dodge);
            melodyController.rigidBody.velocity = Vector3.zero;

            timer = 0;
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);

            timer += time;
            Debug.Log("Timer: " + timer);
            if (timer >= melodyController.config.DashIntroTime)
            {
                ableToExit = true;
            }
        }

        public override void OnExit()
        {
            Debug.Log("DashIntro OnExit");
        }
    }
}
