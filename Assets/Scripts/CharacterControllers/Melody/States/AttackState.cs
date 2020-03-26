namespace Melody.States
{
    public class AttackState : MelodyState
    {
        public AttackState(MelodyController controller) : base(controller) { }

        protected override void Enter()
        {
            melodyController.melodyAnimator.Attack();
            melodyController.melodyHitboxes.ActivateHitbox("PlayerAttack", 0.0f, 0.25f, MelodyStats.attackDamage);
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);
            if (melodyController.melodyAnimator.IsAttackFinishedPlaying())
            {
                nextState = new IdleState(melodyController);
                ableToExit = true;
            }
        }

        public override void OnExit(){}
    }
}