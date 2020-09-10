namespace Melody.States
{
    using HarmonyQuest.Audio;

    public class DashOutroState : MelodyState
    {
        protected float timer = 0;

        public DashOutroState(MelodyController controller) : base(controller) { stateName = "DashOutroState"; }

        protected override void Enter()
        {
            melodyController.melodyAnimator.ExitDash();

            nextState = new IdleState(melodyController);
            timer = 0;
            melodyController.melodyHealth.isDashing = false;
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);

            //Allow Melody to cancel the dash outro into an attack, grapple, or counter
            if (melodyController.input.AttackButtonDown() && FmodFacade.instance.HasPerformedActionThisBeat() == false && melodyController.melodyCollision.IsSliding() == false)
            {
                ableToExit = true;
                nextState = new AttackRequestState(melodyController);
            }
            else if (melodyController.input.GrappleButtonDown() && FmodFacade.instance.HasPerformedActionThisBeat() == false && melodyController.melodyGrappleHook.HasGrappleDestination())
            {
                ableToExit = true;
                nextState = new GrappleHookIntroState(melodyController);
            }
            else if (melodyController.input.ParryButtonDown() && melodyController.melodyCollision.IsSliding() == false)
            {
                ableToExit = true;
                nextState = new CounterState(melodyController);
            }
            else
            {
                timer += time;
                if (timer >= melodyController.config.DashOutroTime)
                {
                    if (melodyController.melodyPhysics.GetVelocity().magnitude >= 0.0f)
                    {
                        nextState = new MovingState(melodyController);
                    }
                    else
                    {
                        nextState = new IdleState(melodyController);
                    }
                    ableToExit = true;
                }
            }

            melodyController.melodyPhysics.CalculateVelocity(melodyController.config.DashOutroMaxSpeed, melodyController.config.MaxAcceleration);
        }

        public override void OnFixedUpdate()
        {
            melodyController.melodyPhysics.ApplyVelocity(melodyController.config.DashOutroMaxSpeed, melodyController.config.DashOutroTurningSpeed);
            melodyController.melodyPhysics.ApplyGravity(melodyController.config.DashOutroGravity);
            melodyController.melodyPhysics.SnapToGround();
            base.OnFixedUpdate();
        }

        public override void OnExit(){}
    }
}
