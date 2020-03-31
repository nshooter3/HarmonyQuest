namespace Melody.States
{
    using HarmonyQuest.Audio;
    using UnityEngine;

    public class AttackRequestState : MelodyState
    {
        public AttackRequestState(MelodyController controller) : base(controller) { }

        FmodFacade.OnBeatAccuracy accuracy;

        protected override void Enter()
        {
            accuracy = FmodFacade.instance.WasActionOnBeat();
            if (accuracy == FmodFacade.OnBeatAccuracy.Great)
            {
                nextState = new AttackSuccessState(melodyController);
            }
            else if (accuracy == FmodFacade.OnBeatAccuracy.Good)
            {
                nextState = new AttackSuccessState(melodyController);
            }
            else
            {
                nextState = new AttackMissState(melodyController);
            }
            ableToExit = true;
        }

        public override void OnExit()
        {

        }
    }
}
