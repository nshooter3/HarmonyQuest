namespace Melody.States
{
    using System;
    using UnityEngine;

    public class DashState : MelodyState
    {

        protected Vector3 dodgeVector = Vector3.left;
        protected float timer = 0;

        public DashState(MelodyController controller, Vector3 dodgeVector) : base(controller)
        {
            this.dodgeVector = dodgeVector;
            Debug.Log("DashState");
        }

        protected override void Enter()
        {
            nextState = new DashOutroState(melodyController);
            timer = 0;
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);

            melodyController.rigidBody.velocity = dodgeVector;

            timer += time;
            if (timer >= melodyController.config.DashTime)
            {
                ableToExit = true;
            }
        }

        public override void OnExit()
        {
        }
    }
}
