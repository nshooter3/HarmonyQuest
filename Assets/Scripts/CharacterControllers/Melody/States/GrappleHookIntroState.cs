﻿namespace Melody.States
{
    using UnityEngine;

    public class GrappleHookIntroState : MelodyState
    {
        public GrappleHookIntroState(MelodyController controller) : base(controller) { stateName = "GrappleHookIntroState"; }

        protected override void Enter()
        {
            nextState = new GrappleHookState(melodyController, melodyController.melodyGrappleHook.GetGrappleDestination());
            melodyController.rigidBody.velocity = Vector3.zero;
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
