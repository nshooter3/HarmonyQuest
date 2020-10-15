using HarmonyQuest.Audio;
using UnityEngine;

namespace HarmonyQuest.Animation.StateMachineBehaviours
{
    public class NormalizeToBeat : StateMachineBehaviour
    {
        public enum LENGTH_IN_BEATS
        {
            QUARTER,
            HALF,
            ONE,
            TWO,
            THREE,
            FOUR,
        }

        public LENGTH_IN_BEATS BeatLength = LENGTH_IN_BEATS.ONE;

        private float TargetBeats;
        private float LastBeatProgressUpdate;
        private float BeatsCounter;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            switch (BeatLength)
            {
                case LENGTH_IN_BEATS.QUARTER:
                    TargetBeats = 0.25f;
                    break;
                case LENGTH_IN_BEATS.HALF:
                    TargetBeats = 0.5f;
                    break;
                case LENGTH_IN_BEATS.ONE:
                    TargetBeats = 1f;
                    break;
                case LENGTH_IN_BEATS.TWO:
                    TargetBeats = 2f;
                    break;
                case LENGTH_IN_BEATS.THREE:
                    TargetBeats = 3f;
                    break;
                case LENGTH_IN_BEATS.FOUR:
                    TargetBeats = 4f;
                    break;
            }
            BeatsCounter = 0f;
            LastBeatProgressUpdate = FmodFacade.instance.GetNormalizedBeatProgress();
            animator.Play(0, -1, BeatsCounter / TargetBeats);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!PauseManager.GetPaused())
            {
                if (FmodFacade.instance.GetNormalizedBeatProgress() > LastBeatProgressUpdate)
                {
                    BeatsCounter += FmodFacade.instance.GetNormalizedBeatProgress() - LastBeatProgressUpdate;
                }
                else
                {
                    BeatsCounter += 1f - LastBeatProgressUpdate + FmodFacade.instance.GetNormalizedBeatProgress();
                }
                LastBeatProgressUpdate = FmodFacade.instance.GetNormalizedBeatProgress();
                BeatsCounter = Mathf.Clamp(BeatsCounter, 0, TargetBeats);
                animator.Play(0, -1, BeatsCounter / TargetBeats);

                if (BeatsCounter == TargetBeats)
                {
                    BeatsCounter = 0;
                }
            }
            else
            {
                animator.enabled = false;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}
