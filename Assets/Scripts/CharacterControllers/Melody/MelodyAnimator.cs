namespace Melody
{
    using UnityEngine;

    public class MelodyAnimator
    {
        private MelodyController controller;

        public MelodyAnimator(MelodyController controller)
        {
            this.controller = controller;
        }

        public void SetWalkRun(float percentageOfMax)
        {
            controller.animator.SetFloat("Move", percentageOfMax);
        }

        public void Attack()
        {
            controller.animator.SetTrigger("Attack");
        }

        public bool IsAttackFinishedPlaying()
        {
            bool isFinished = controller.animator.IsInTransition(0) && controller.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
            if (isFinished)
            {
                controller.animator.ResetTrigger("Attack");

            }
            return isFinished;
        }

        public void Counter()
        {
            controller.animator.SetTrigger("Counter");
        }

        public bool IsCounterFinishedPlaying()
        {
            bool isFinished = controller.animator.IsInTransition(0) && controller.animator.GetCurrentAnimatorStateInfo(0).IsName("Counter");
            if (isFinished)
            {
                controller.animator.ResetTrigger("Counter");

            }
            return isFinished;
        }

        public void EnterDash()
        {
            controller.melodyRenderer.enabled = false;
            controller.scarfRenderer.enabled = true;
        }

        public void ExitDash()
        {
            controller.melodyRenderer.enabled = true;
            controller.scarfRenderer.enabled = false;
        }
    }
}
