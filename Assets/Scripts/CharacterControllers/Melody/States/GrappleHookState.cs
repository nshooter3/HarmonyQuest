namespace Melody.States
{
    using Objects;
    using UnityEngine;

    public class GrappleHookState : MelodyState
    {
        public GrappleHookState(MelodyController controller, GrapplePoint destination) : base(controller)
        {
            this.destination = destination;
            startPos = melodyController.transform.position;
            stateName = "GrappleHookState";
        }

        float totalGrappleTime = 0.15f;
        float curGrappleTime = 0f;

        Vector3 startPos;
        GrapplePoint destination;

        protected override void Enter()
        {
            
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);
            curGrappleTime = Mathf.Min(curGrappleTime + time, totalGrappleTime);
            melodyController.melodyPhysics.SetPosition(Vector3.Lerp(startPos, destination.actualDestination.position, curGrappleTime/totalGrappleTime));

            if (curGrappleTime >= totalGrappleTime)
            {
                nextState = new GrappleHookOutroState(melodyController);
                ableToExit = true;
            }
        }

        public override void OnExit()
        {
            destination.StartCooldownTimer();
        }
    }
}
