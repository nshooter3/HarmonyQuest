﻿namespace Melody.States
{
    public class AttackState : MelodyState
    {
        public AttackState(MelodyController controller) : base(controller) { }

        protected override void Enter()
        {
            melodyController.animator.SetTrigger("Attack");
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);
            if (melodyController.animator.IsInTransition(0) && melodyController.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                nextState = new IdleState(melodyController);
                ableToExit = true;
            }
        }

        public override void OnExit()
        {
            melodyController.animator.ResetTrigger("Attack");
        }
    }
}