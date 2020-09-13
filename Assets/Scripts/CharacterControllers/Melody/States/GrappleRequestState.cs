namespace Melody.States
{
    using HarmonyQuest.Audio;

    public class GrappleRequestState : MelodyState
    {
        public GrappleRequestState(MelodyController controller) : base(controller) { stateName = "GrappleRequestState"; }

        FmodFacade.OnBeatAccuracy accuracy;

        protected override void Enter()
        {
            accuracy = FmodFacade.instance.WasActionOnBeat();
            if (accuracy == FmodFacade.OnBeatAccuracy.Great)
            {
                nextState = new GrappleHookIntroState(melodyController);
            }
            else if (accuracy == FmodFacade.OnBeatAccuracy.Good)
            {
                nextState = new GrappleHookIntroState(melodyController);
            }
            else
            {
                nextState = new GrappleMissState(melodyController);
            }
            ableToExit = true;
            FmodFacade.instance.PerformOnBeatAction();

            melodyController.melodyAnimator.SwitchJabArm();

            melodyController.melodyPhysics.CalculateVelocity(melodyController.config.AttackMaxSpeed, melodyController.config.AttackMaxAcceleration);
        }

        public override void OnFixedUpdate()
        {
            melodyController.melodyPhysics.ApplyVelocity(melodyController.config.MaxSpeed, melodyController.config.TurningSpeed);
            melodyController.melodyPhysics.ApplyGravity(melodyController.config.Gravity);
            melodyController.melodyPhysics.SnapToGround();
            base.OnFixedUpdate();
        }

        public override void OnExit()
        {

        }
    }
}
