namespace Melody.States
{
    using HarmonyQuest.Audio;
    using Objects;
    using UnityEngine;

    public class GrappleHookIntroState : MelodyState
    {
        GrapplePoint destination;

        public GrappleHookIntroState(MelodyController controller, GrapplePoint destination) : base(controller)
        {
            stateName = "GrappleHookIntroState";
            this.destination = destination;
        }

        protected override void Enter()
        {
            nextState = new GrappleHookState(melodyController, destination);
            melodyController.rigidBody.velocity = Vector3.zero;
            FmodFacade.instance.PerformOnBeatAction();
            ableToExit = true;
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
