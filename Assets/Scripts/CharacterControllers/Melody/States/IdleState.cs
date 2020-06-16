namespace Melody.States
{
    using HarmonyQuest.Audio;

    public class IdleState : MelodyState
    {

        public IdleState(MelodyController controller) : base(controller) { stateName = "IdleState"; }

        protected override void Enter() { }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);

            if (melodyController.input.AttackButtonDown() && FmodFacade.instance.HasPerformedActionThisBeat() == false && melodyController.melodyCollision.IsSliding() == false)
            {

                ableToExit = true;
                nextState = new AttackRequestState(melodyController);
            }
            else if (melodyController.input.ParryButtonDown() && melodyController.melodyCollision.IsSliding() == false)
            {
                ableToExit = true;
                nextState = new CounterState(melodyController);
            }
            else if (melodyController.input.DodgeButtonDown() && melodyController.melodyCollision.IsSliding() == false)
            {
                ableToExit = true;
                nextState = new DashIntroState(melodyController);
            }
            else if (melodyController.input.GetHorizontalMovement() != 0 || melodyController.input.GetVerticalMovement() != 0)
            {
                ableToExit = true;
                nextState = new MovingState(melodyController);
            }
        }

        public override void OnFixedUpdate()
        {
            if (melodyController.melodyCollision.IsGrounded() && !melodyController.melodyCollision.IsSliding())
            {
                melodyController.melodyPhysics.IgnoreHorizontalMovementInput();
                melodyController.melodyPhysics.ApplyVelocity(melodyController.config.MaxSpeed, melodyController.config.TurningSpeed);
            }
            melodyController.melodyPhysics.ApplyGravity(melodyController.config.Gravity, true);
            if (melodyController.melodyLockOn.HasLockonTarget() == true)
            {
                melodyController.melodyPhysics.RotatePlayer(melodyController.config.TurningSpeed, true);
            }
            base.OnFixedUpdate();
        }

        public override void OnExit() { }
    }
}
