namespace Melody
{
    using UnityEngine;

    public class MelodyCollision
    {
        private MelodyController controller;

        private bool isGrounded;
        private bool isSliding;

        //Radian Y value of the steepest slope that can cause the player to slide.
        private float steepestSlopeYAngle;
        //Collision normal direction of the steepest slope, used to calculate which direction the player should slide.
        public Vector3 steepestSlopeDirection { get; private set; }

        public MelodyCollision(MelodyController controller)
        {
            this.controller = controller;
            this.controller.melodyColliderWrapper.AssignFunctionToCollisionEnterDelegate(SetGrounded);
            this.controller.melodyColliderWrapper.AssignFunctionToCollisionStayDelegate(SetGrounded);
        }

        public void OnFixedUpdate()
        {
            //Since FixedUpdate fires before collision functions, we can reset isGrounded here and let our collision enter/stay functions recheck every frame.
            isGrounded = false;
            isSliding = false;
        }

        public void SetGrounded(Collision collision)
        {
            steepestSlopeYAngle = 0;
            steepestSlopeDirection = Vector3.zero;
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;
                Vector3 normalPerpendicular = Vector3.Cross(collision.GetContact(i).normal, Vector3.up).normalized;
                //For determing the slope Melody is standing on, compare her contact normals to the up angle.
                float slopeDownAngle = Vector3.Angle(normal, Vector3.up);
                //If Melody is colliding with something that has a contact normal y angle less than groundedYAngleCutoff, we consider her grounded.
                isGrounded |= slopeDownAngle <= controller.config.groundedYAngleCutoff;
                isSliding |= slopeDownAngle > controller.config.groundedYAngleCutoff && slopeDownAngle <= controller.config.slidingYAngleCutoff;
                //If this slope is steeper than our last one without angling further downwards than sideways, then it is our new steepest slope.
                if (slopeDownAngle > steepestSlopeYAngle && slopeDownAngle < controller.config.slidingYAngleCutoff)
                {
                    steepestSlopeYAngle = slopeDownAngle;
                    steepestSlopeDirection = normal;
                }
            }
            Debug.Log("SLOPE ANGLE: " + steepestSlopeYAngle);
        }

        public bool IsGrounded()
        {
            return isGrounded;
        }

        public bool IsSliding()
        {
            return isSliding && !isGrounded;
        }

        public bool IsInAir()
        {
            return !isGrounded && !isSliding;
        }
    }
}
