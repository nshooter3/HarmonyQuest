namespace Melody.States
{
    using HarmonyQuest.Audio;
    using UnityEngine;

    public class MovingState : MelodyState
    {
        public MovingState(MelodyController controller) : base(controller) { stateName = "MovingState"; }

        protected override void Enter()
        {
            
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);

            //Check For Attack
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
            else if (melodyController.move.magnitude == 0.0f)
            {
                ableToExit = true;
                nextState = new IdleState(melodyController);
            }

            melodyController.melodyPhysics.CalculateVelocity(melodyController.config.MaxSpeed, melodyController.config.MaxAcceleration);
        }

        public override void OnFixedUpdate()
        {
            melodyController.melodyPhysics.ApplyVelocity(melodyController.config.MaxSpeed, melodyController.config.TurningSpeed, true);
            if (melodyController.move.magnitude == 0.0f && melodyController.melodyCollision.IsGrounded() == true)
            {
                //If there is no controller input and melody is grounded, do not apply gravity. This prevents her from infinitely sliding down hills.
            }
            else
            {
                melodyController.melodyPhysics.ApplyGravity(melodyController.config.Gravity);
            }
            melodyController.melodyPhysics.SnapToGround();

            //Debug.Log("Moving State Velocity Magnitude: " + melodyController.melodyPhysics.velocity.magnitude);

            base.OnFixedUpdate();
        }

        public override void OnExit()
        {
            
        }
    }
}