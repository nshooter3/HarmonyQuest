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
        public Vector3 steepestSlopeNormal { get; private set; }
        //The vector that travels up or down along the collision plane. Follows the slope of the collision plane.
        public Vector3 steepestSlopeNormalPerpendicular { get; private set; }
        //The vector that travels horizontally along the collision plane. Has a y value of 0.
        public Vector3 steepestSlopeNormalPerpendicularSide { get; private set; }
        //Dot product of the player's forward and the slope normal. When this is greater than 0, it means the player is moving towards the direction the slope is pushing them.
        //When this is the case, we want to apply gravity to prevent the player from jittering as they go downhill.
        public float slopeNormalDotProduct;

        private readonly bool debug = true;

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
            slopeNormalDotProduct = 0;
        }

        public void SetGrounded(Collision collision)
        {
            steepestSlopeYAngle = 0;
            steepestSlopeNormal = Vector3.zero;
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;
                Vector3 normal2D = normal;
                normal2D.y = 0;

                //Get a vector that travels sideways along the collision plane. Has a y value of 0.
                Vector3 normalPerpendicularSide = Vector3.Cross(normal, normal2D).normalized;
                //Use the collision normal and the sideways perpendicular vector to get a new perpendicular vector that follows the slope of our collision plane.
                Vector3 normalPerpendicular = Vector3.Cross(normalPerpendicularSide, normal).normalized;
                //For determing the slope Melody is standing on, compare her contact normals to the up angle.
                float slopeDownAngle = Vector3.Angle(normal, Vector3.up);

                //If Melody is colliding with something that has a contact normal y angle less than groundedYAngleCutoff, we consider her grounded.
                isGrounded |= slopeDownAngle <= controller.config.groundedYAngleCutoff;
                //If Melody is colliding with something that has a contact normal y angle less than slopeDownAngle but greater than groundedYAngleCutoff, we consider her sliding.
                isSliding |= slopeDownAngle > controller.config.groundedYAngleCutoff && slopeDownAngle <= controller.config.slidingYAngleCutoff;

                //If this slope is steeper than our last one without angling further downwards than sideways, then it is our new steepest slope.
                if (slopeDownAngle > steepestSlopeYAngle && slopeDownAngle < controller.config.slidingYAngleCutoff)
                {
                    steepestSlopeYAngle = slopeDownAngle;
                    steepestSlopeNormal = normal;
                    slopeNormalDotProduct = Vector3.Dot(controller.GetTransformForward(), steepestSlopeNormal);
                    steepestSlopeNormalPerpendicularSide = normalPerpendicularSide;
                    steepestSlopeNormalPerpendicular = normalPerpendicular;
                }
            }
            if (debug && (isGrounded || isSliding))
            {
                Debug.DrawRay(controller.GetTransform().position, steepestSlopeNormal, Color.yellow);
                Debug.DrawRay(controller.GetTransform().position, steepestSlopeNormalPerpendicularSide, Color.blue);
                Debug.DrawRay(controller.GetTransform().position, steepestSlopeNormalPerpendicular, Color.green);
            }
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
