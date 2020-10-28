namespace Melody.States
{
    public class GrappleRequestState : MelodyState
    {
        public GrappleRequestState(MelodyController controller) : base(controller) { stateName = "GrappleRequestState"; }

        protected override void Enter()
        {
            if (melodyController.melodyGrappleHook.HasGrappleDestination())
            {
                nextState = new GrappleHookIntroState(melodyController, melodyController.melodyGrappleHook.GetGrappleDestination());
            }
            else
            {
                nextState = new GrappleMissState(melodyController);
            }
            ableToExit = true;

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
