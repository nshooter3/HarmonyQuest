namespace Melody
{
    using Objects;
    using UnityEngine;

    public class MelodyRamp
    {
        private MelodyController controller;

        [HideInInspector]
        public bool isRampDash = false;
        [HideInInspector]
        public float dashSpeedMultiplier;
        [HideInInspector]
        public float dashDurationMultiplier;
        [HideInInspector]
        public Vector3 rampDirection;

        private float dashAlongRampDegreeRange;
        private Vector3 raycastOrigin;
        private RaycastHit hit;
        private float groundCheckRaycastYOffset;
        private LayerMask rampLayerMask;

        public MelodyRamp(MelodyController controller)
        {
            this.controller = controller;
            groundCheckRaycastYOffset = controller.config.groundCheckRaycastYOffset;
            rampLayerMask = controller.config.rampLayerMask;
            dashAlongRampDegreeRange = controller.config.dashAlongRampDegreeRange;

        }

        public void OnFixedUpdate()
        {
            if (IsMelodyOnRamp(controller.transform.position, controller.config.groundCheckRaycastDistance))
            {
                if (!isRampDash && controller.melodyPhysics.isDashing)
                {
                    //The ramp will attempt to launch Melody in the direction of its normal.
                    rampDirection = hit.transform.forward;
                    if (IsMelodyDashingAlongRamp(controller.melodyPhysics.dashDirection, rampDirection))
                    {
                        isRampDash = true;
                        dashSpeedMultiplier = hit.transform.GetComponent<Ramp>().dashSpeedMultiplier;
                        dashDurationMultiplier = hit.transform.GetComponent<Ramp>().dashDurationMultiplier;
                    }
                }
            }
        }

        protected bool IsMelodyOnRamp(Vector3 origin, float distance)
        {
            raycastOrigin = new Vector3(origin.x, origin.y + groundCheckRaycastYOffset, origin.z);
            return Physics.Raycast(raycastOrigin, -Vector3.up, out hit, distance + groundCheckRaycastYOffset, rampLayerMask);
        }

        //Check if Melody's dash lines up with the direction the ramp is facing
        private bool IsMelodyDashingAlongRamp(Vector3 dashDirection, Vector3 rampDirection)
        {
            dashDirection.y = 0f;
            rampDirection.y = 0f;
            return Vector3.Angle(dashDirection, rampDirection) <= dashAlongRampDegreeRange;
        }
    }
}
