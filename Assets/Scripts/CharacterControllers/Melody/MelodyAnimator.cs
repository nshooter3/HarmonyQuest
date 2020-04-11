namespace Melody
{
    using System;
    using UnityEngine;

    public class MelodyAnimator
    {
        private MelodyController controller;

        private readonly int[] animationHashes;

        //These names must match the name of the animation in the AnimationController.
        public enum Animations
        {
            Move =    0,
            Attack =  1,
            Counter = 2, 
        }

        public MelodyAnimator(MelodyController controller)
        {
            this.controller = controller;

            string[] names = Enum.GetNames(typeof(Animations));
            animationHashes = new int[names.Length];
            for(int i = 0; i < names.Length; i++)
            {
                animationHashes[i] = Animator.StringToHash(names[i]);
            }
        }

        public void SetWalkRun(float percentageOfMax)
        {
            controller.animator.SetFloat(animationHashes[(int) Animations.Move], percentageOfMax);
        }

        public void PlayAnimation(Animations animation)
        {
            controller.animator.SetTrigger(animationHashes[(int) animation]);
        }

        public bool IsAnimationDonePlaying(Animations animation)
        {
            bool isFinished = controller.animator.IsInTransition(0) && controller.animator.GetCurrentAnimatorStateInfo(0).shortNameHash == (int) animation;
            if (isFinished)
            {
                controller.animator.ResetTrigger(animationHashes[(int) animation]);

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
