using UnityEngine;

namespace Melody.States
{
    public class SuccessfulCounterState : MelodyState
    {
        public SuccessfulCounterState(MelodyController controller) : base(controller) { stateName = "SuccessfulCounterState"; }

        float cooldownTimer = float.MaxValue;

        protected override void Enter()
        {
            Debug.Log("Successful counter!");
            melodyController.melodyHealth.SetPostSuccessfulCounterTimer();
            cooldownTimer = melodyController.config.SuccessfulCounterCooldownTime;
            melodyController.melodySound.CounterSuccess();
        }

        public override void OnUpdate(float time)
        {
            cooldownTimer -= time;
            if (cooldownTimer <= 0)
            {
                Debug.Log("Exit successful counter state");
                nextState = new IdleState(melodyController);
                ableToExit = true;
            }
            base.OnUpdate(time);
        }

        public override void OnFixedUpdate()
        {
            melodyController.melodyPhysics.ApplyStationaryVelocity();
            melodyController.melodyPhysics.ApplyGravity(melodyController.config.CounterGravity);
            melodyController.melodyPhysics.SnapToGround();
            base.OnFixedUpdate();
        }

        public override void OnExit()
        {
            
        }
    }
}
