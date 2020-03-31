namespace Melody.States
{
    using UnityEngine;

    public class CounterState : MelodyState
    {
        public CounterState(MelodyController controller) : base(controller) { }

        //Since the placeholder animation is so long, use a timer to exit this state sooner.
        float tempTimer = 0.5f;

        //How long the counter is active. During this time, the counter hurtbox is active, and any recieved damage will be turned back on the enemy.
        float counterActiveTimer = 0.25f;

        bool counterActive = true;

        protected override void Enter()
        {
            melodyController.melodyAnimator.Counter();
            melodyController.counterHurtbox.enabled = true;
            melodyController.counterHurtboxMesh.enabled = true;
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);

            if (counterActive == true)
            {
                if (counterActiveTimer <= 0.0f)
                {
                    counterActive = false;
                    melodyController.counterHurtbox.enabled = false;
                    melodyController.counterHurtboxMesh.enabled = false;
                }
                else
                {
                    counterActiveTimer -= Time.deltaTime;
                }
            }

            tempTimer -= Time.deltaTime;
            if (melodyController.melodyAnimator.IsCounterFinishedPlaying() || tempTimer <= 0.0f)
            {
                nextState = new IdleState(melodyController);
                ableToExit = true;
            }
            melodyController.melodyPhysics.CalculateVelocity(melodyController.config.CounterMaxSpeed, melodyController.config.CounterMaxAcceleration);
        }

        public override void OnFixedUpdate()
        {
            melodyController.melodyPhysics.ApplyVelocity(melodyController.config.CounterMaxSpeed, melodyController.config.CounterTurningSpeed);
            melodyController.melodyPhysics.ApplyGravity(melodyController.config.CounterGravity);
            melodyController.melodyPhysics.SnapToGround();
            base.OnFixedUpdate();
        }

        public override void OnExit()
        {
            melodyController.counterHurtbox.enabled = false;
            melodyController.counterHurtboxMesh.enabled = false;
        }
    }
}
