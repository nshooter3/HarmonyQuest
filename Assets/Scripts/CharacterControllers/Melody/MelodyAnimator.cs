﻿namespace Melody
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

        private Vector2 forward2D;
        private Vector2 velocity2D;

        public MelodyAnimator(MelodyController controller)
        {
            this.controller = controller;
            forward2D = new Vector2();
            velocity2D = new Vector2();

            string[] names = Enum.GetNames(typeof(Animations));
            animationHashes = new int[names.Length];
            for(int i = 0; i < names.Length; i++)
            {
                animationHashes[i] = Animator.StringToHash(names[i]);
            }
        }

        public void SetWalkRun(float percentageOfMax)
        {
            controller.Animator.SetFloat(animationHashes[(int) Animations.Move], percentageOfMax);
        }

        public void SetStrafeInfo(Vector3 forward, Vector3 velocity)
        {
            forward2D.Set(forward.x, forward.z);
            velocity2D.Set(velocity.x, velocity.z);

            if (velocity.magnitude > 0.0f)
            {
                controller.Animator.SetFloat("ForwardBackward", Mathf.Cos(Mathf.Deg2Rad * Vector2.SignedAngle(forward2D, velocity2D)));
                controller.Animator.SetFloat("RightLeft", Mathf.Sin(Mathf.Deg2Rad * Vector2.SignedAngle(forward2D, velocity2D)));
            }
            else
            {
                controller.Animator.SetFloat("ForwardBackward", 0);
                controller.Animator.SetFloat("RightLeft", 0);
            }
        }

        public void SetBoolParam(string param, bool val)
        {
            controller.Animator.SetBool(param, val);
        }

        public bool GetBoolParam(string param)
        {
            return controller.Animator.GetBool(param);
        }

        public void SwitchJabArm()
        {
            controller.Animator.SetBool("RightJab", !controller.Animator.GetBool("RightJab"));
        }

        public void PlayAnimation(Animations animation)
        {
            controller.Animator.SetTrigger(animationHashes[(int) animation]);
        }

        public bool IsAnimationDonePlaying(Animations animation)
        {
            bool isFinished = controller.Animator.IsInTransition(0) && controller.Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == (int) animation;
            if (isFinished)
            {
                controller.Animator.ResetTrigger(animationHashes[(int) animation]);

            }
            return isFinished;
        }

        public void EnterDash()
        {
            controller.melodyRenderer.SetActive(false);
            controller.scarfRenderer.enabled = true;
        }

        public void ExitDash()
        {
            controller.melodyRenderer.SetActive(true);
            controller.scarfRenderer.enabled = false;
        }
    }
}
