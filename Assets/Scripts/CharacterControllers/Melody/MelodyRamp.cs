namespace Melody
{
    using UnityEngine;

    public class MelodyRamp
    {
        private MelodyController controller;

        [HideInInspector]
        public bool isRampDash = false;

        //Used to determine whether or not Melody is dashing relatively in the same direction as the ramp.
        private float dashAlongRampDegreeRange = 60f;

        private Vector3 rampDirection;

        private string rampLayerName = "Ramp";

        public MelodyRamp(MelodyController controller)
        {
            this.controller = controller;
            controller.melodyColliderWrapper.AssignFunctionToCollisionEnterDelegate(CheckIfOnRamp);
            controller.melodyColliderWrapper.AssignFunctionToCollisionStayDelegate(CheckIfOnRamp);
        }

        private void CheckIfOnRamp(Collision collision)
        {
            if (!isRampDash && controller.melodyPhysics.isDashing && LayerMask.LayerToName(collision.gameObject.layer) == rampLayerName)
            {
                rampDirection = collision.transform.forward;
                if (IsMelodyDashingAlongRamp(controller.melodyPhysics.dashDirection, rampDirection))
                {
                    isRampDash = true;
                    Debug.Log("RAMP DASH!");
                }
            }
        }

        private bool IsMelodyDashingAlongRamp(Vector3 dashDirection, Vector3 rampDirection)
        {
            return Vector3.Angle(dashDirection, rampDirection) <= dashAlongRampDegreeRange;
        }
    }
}
