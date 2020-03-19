namespace Melody.States
{
    using UnityEngine;

    public class DashState : MelodyState
    {

        protected Vector3 dodge;
        protected float timer = 0.0f;

        public DashState(MelodyController controller, Vector3 dodge) : base(controller)
        {
            this.dodge = dodge;
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
        }

        public override void OnFixedUpdate()
        {
            melodyController.melodyPhysics.SnapToGround();
            melodyController.rigidBody.velocity = dodge;
            base.OnFixedUpdate();
        }

        public override void OnExit()
        {

        }
    }
}
