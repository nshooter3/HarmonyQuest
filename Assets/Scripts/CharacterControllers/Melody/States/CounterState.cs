namespace Melody.States
{
    using UnityEngine;

    public class CounterState : MelodyState
    {
        public CounterState(MelodyController controller) : base(controller) { stateName = "CounterState"; }

        //Since the placeholder animation is so long, use a timer to exit this state sooner.
        float tempTimer = 0.6f;

        //How long the counter is active. During this time, the counter hurtbox is active, and any recieved damage will be turned back on the enemy.
        float counterActiveTimer;

        bool counterActive = true;

        protected override void Enter()
        {
            counterActiveTimer = melodyController.config.PreCounterGracePeriod;
            melodyController.melodyAnimator.PlayAnimation(MelodyAnimator.Animations.Counter);
            melodyController.counterHurtbox.enabled = true;
            //melodyController.counterHurtboxMesh.enabled = true;
            melodyController.melodyHealth.isCountering = true;
            melodyController.melodyHealth.dealtCounterDamage = false;
            melodyController.melodyHealth.CheckForLateCounters();
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);

            if (melodyController.melodyHealth.dealtCounterDamage == true)
            {
                nextState = new SuccessfulCounterState(melodyController);
                ableToExit = true;
                return;
            }

            if (counterActive == true)
            {
                if (counterActiveTimer <= 0.0f)
                {
                    counterActive = false;
                    EndCounter();
                }
                else
                {
                    counterActiveTimer -= Time.deltaTime;
                }
            }

            tempTimer -= Time.deltaTime;
            if (melodyController.melodyAnimator.IsAnimationDonePlaying(MelodyAnimator.Animations.Counter) || tempTimer <= 0.0f)
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
            EndCounter();
        }

        private void EndCounter()
        {
            melodyController.counterHurtbox.enabled = false;
            //melodyController.counterHurtboxMesh.enabled = false;
            melodyController.melodyHealth.isCountering = false;
            melodyController.melodyHealth.dealtCounterDamage = false;
        }
    }
}
