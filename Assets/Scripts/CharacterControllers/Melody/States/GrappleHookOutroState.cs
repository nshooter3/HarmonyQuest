namespace Melody.States
{
    using HarmonyQuest.Audio;

    public class GrappleHookOutroState : MelodyState
    {
        protected float timer = 0;

        public GrappleHookOutroState(MelodyController controller) : base(controller) { stateName = "GrappleHookOutroState"; }

        protected override void Enter()
        {
            
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);

            //Allow Melody to cancel the dash outro into an attack, dash, or counter
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
            else
            {
                timer += time;
                if (timer >= melodyController.config.grappleOutroTime)
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

            melodyController.melodyPhysics.CalculateVelocity(melodyController.config.grappleOutroMaxSpeed, melodyController.config.MaxAcceleration);
        }

        public override void OnExit()
        {
            melodyController.melodyPhysics.ApplyVelocity(melodyController.config.grappleOutroMaxSpeed, melodyController.config.grappleOutroTurningSpeed);
            melodyController.melodyPhysics.ApplyGravity(melodyController.config.grappleOutroGravity);
            melodyController.melodyPhysics.SnapToGround();
            base.OnFixedUpdate();
        }
    }
}