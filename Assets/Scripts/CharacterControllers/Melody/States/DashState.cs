namespace Melody.States
{
    using System;
    using UnityEngine;

    public class DashState : MelodyState
    {

        protected Vector3 dodgeVector = Vector3.left;
        protected float timer = 0;
        private Vector3 shift;

        public DashState(MelodyController controller, Vector3 dodgeVector) : base(controller)
        {
            this.dodgeVector = dodgeVector;
            shift = new Vector3( 0f, 0.05f, 0 );
        }

        protected override void Enter()
        {
            nextState = new DashOutroState(melodyController);
            timer = 0;
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);

            timer += time;
            if (timer >= melodyController.config.DashTime)
            {
                ableToExit = true;
            }
        }

        public override void OnFixedUpdate()
        {
            RaycastHit[] hits = Physics.RaycastAll(melodyController.transform.position, Vector3.down, 1f);
            Vector3 closest = melodyController.transform.position;
            float min = 1000;
            foreach (RaycastHit h in hits)
            {
                if (min > Math.Abs(melodyController.transform.position.y - h.point.y))
                {
                    min = Math.Abs(melodyController.transform.position.y - h.point.y);
                    closest = h.point+ shift;
                }
            }
            if(min > 0.3f)
            {
                melodyController.transform.position = closest;
            }
            melodyController.rigidBody.velocity = dodgeVector;
            base.OnFixedUpdate();
        }

        public override void OnExit()
        {
        }
    }
}
