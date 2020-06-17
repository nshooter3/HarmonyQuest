namespace Melody
{
    using UnityEngine;

    public class MelodyCollision
    {
        private MelodyController controller;

        //Bools used to toggle which type of check we use to determine the slope of the surface melody is standing on.
        //Option 1: Use the highest normal value from the ground colliding with Melody.
        private bool useCollisionNormals = false;
        //Option 2: Use the averaged normals from a series of raycasts from Melody's feet to the ground.
        private bool useGroupRaycastNormals = true;

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

        //Variables used to calculate floor normals.
        Vector3 raycastOrigin;
        RaycastHit hit;
        Vector3 hitNormal;
        Vector3 averagedHitNormal;

        int numRaycastHits = 0;

        private readonly bool debug = true;

        public MelodyCollision(MelodyController controller)
        {
            this.controller = controller;
            if (useCollisionNormals == true)
            {
                this.controller.melodyColliderWrapper.AssignFunctionToCollisionEnterDelegate(SetGroundedFromCollision);
                this.controller.melodyColliderWrapper.AssignFunctionToCollisionStayDelegate(SetGroundedFromCollision);
            }
        }

        public void OnFixedUpdate()
        {
            isGrounded = false;
            isSliding = false;
            slopeNormalDotProduct = 0;
            if (useGroupRaycastNormals == true)
            {
                GetAveragedNormal();
            }
        }

        public void GetAveragedNormal()
        {
            numRaycastHits = 0;
            averagedHitNormal = GetNormalFromRaycast(controller.transform.position, 0f, 0f, controller.config.groundCheckRaycastDistance) * controller.config.groundCheckCenterWeight +
            GetNormalFromRaycast(controller.transform.position,  controller.config.groundCheckRaycastSpread, 0f, controller.config.groundCheckRaycastDistance) +
            GetNormalFromRaycast(controller.transform.position, -controller.config.groundCheckRaycastSpread, 0f, controller.config.groundCheckRaycastDistance) +
            GetNormalFromRaycast(controller.transform.position, 0f,  controller.config.groundCheckRaycastSpread, controller.config.groundCheckRaycastDistance) +
            GetNormalFromRaycast(controller.transform.position, 0f, -controller.config.groundCheckRaycastSpread, controller.config.groundCheckRaycastDistance);
            if (numRaycastHits != 0)
            {
                averagedHitNormal = averagedHitNormal / numRaycastHits;
                SetGroundedFromNormal(averagedHitNormal);
            }
        }

        public Vector3 GetNormalFromRaycast(Vector3 origin, float xOffset, float zOffset, float distance)
        {
            raycastOrigin = new Vector3(origin.x + xOffset, origin.y + controller.config.groundCheckRaycastYOffset, origin.z + zOffset);
            Debug.DrawRay(raycastOrigin, Vector3.down * (distance + controller.config.groundCheckRaycastYOffset), Color.magenta);
            if (Physics.Raycast(raycastOrigin, -Vector3.up, out hit, distance + controller.config.groundCheckRaycastYOffset, controller.config.groundLayerMask))
            {
                hitNormal = hit.normal;
                //Only pay attention to collisions that 
                if (Vector3.Angle(hitNormal, Vector3.up) <= controller.config.slidingYAngleCutoff)
                {
                    numRaycastHits++;
                    return hit.normal;
                }
            }
            return Vector3.zero;
        }

        public void SetGroundedFromCollision(Collision collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                SetGroundedFromNormal(collision.GetContact(i).normal);
            }
        }

        public void SetGroundedFromNormal(Vector3 normal)
        {
            steepestSlopeYAngle = 0;
            steepestSlopeNormal = Vector3.zero;

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
