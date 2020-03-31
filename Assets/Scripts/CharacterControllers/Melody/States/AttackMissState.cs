namespace Melody.States
{
    using UnityEngine;

    public class AttackMissState : MelodyState
    {
        public AttackMissState(MelodyController controller) : base(controller) { }

        //Since the placeholder animation is so long, use a timer to exit this state sooner.
        float tempTimer = 0.2f;

        protected override void Enter()
        {
            melodyController.melodyAnimator.Attack();
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);
            tempTimer -= Time.deltaTime;
            if (melodyController.melodyAnimator.IsAttackFinishedPlaying() || tempTimer <= 0.0f)
            {
                nextState = new IdleState(melodyController);
                ableToExit = true;
            }

            melodyController.melodyPhysics.CalculateVelocity(melodyController.config.AttackMaxSpeed, melodyController.config.AttackMaxAcceleration);
        }

        public override void OnFixedUpdate()
        {
            melodyController.melodyPhysics.ApplyVelocity(melodyController.config.AttackMaxSpeed, melodyController.config.AttackTurningSpeed);
            melodyController.melodyPhysics.ApplyGravity(melodyController.config.AttackGravity);
            melodyController.melodyPhysics.SnapToGround();
            base.OnFixedUpdate();
        }

        public override void OnExit() { }
    }
}
