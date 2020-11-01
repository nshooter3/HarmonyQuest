﻿namespace GamePhysics
{
    using UnityEngine;

    public class SurfaceCollisionEntity
    {
        protected GameObject gameObject;
        protected PhysicsEntity physicsEntity;
        protected CollisionWrapper collisionWrapper;

        //Bools used to toggle which type of check we use to determine the slope of the surface the entity is standing on.
        //Option 1: Use the highest normal value from the ground colliding with the entity.
        protected bool useCollisionNormals;
        //Option 2: Use the averaged normals from a series of raycasts from the entity's feet to the ground.
        protected bool useGroupRaycastNormals;

        protected bool isGrounded;
        protected bool isSliding;

        //Angle Y value of the steepest slope that can cause the player to slide.
        protected float steepestSlopeYAngle;
        //Collision normal direction of the steepest slope, used to calculate which direction the player should slide.
        public Vector3 steepestSlopeNormal { get; private set; }
        //The vector that travels up or down along the collision plane. Follows the slope of the collision plane.
        public Vector3 steepestSlopeNormalPerpendicular { get; private set; }
        //The vector that travels horizontally along the collision plane. Has a y value of 0.
        public Vector3 steepestSlopeNormalPerpendicularSide { get; private set; }
        //A 2D representation of which direction the slope should be pushing back on the player.
        public Vector3 slopeNormal2D;
        //Dot product of the player's forward and the slope normal. When this is greater than 0, it means the player is moving towards the direction the slope is pushing them.
        //When this is the case, we want to apply gravity to prevent the player from jittering as they go downhill.
        public float slopeNormalDotProduct;
        protected float slidingYAngleCutoff;
        protected float groundedYAngleCutoff;
        protected bool canSlide;

        //Variables used to calculate floor normals.
        protected Vector3 raycastOrigin;
        protected RaycastHit hit;
        protected Vector3 hitNormal;
        protected Vector3 averagedHitNormal;

        //Group Raycast vars
        protected float groundCheckRaycastDistance;
        protected float groundCheckRaycastSpread;
        protected float groundCheckCenterWeight;
        protected float groundCheckRaycastYOffset;
        protected LayerMask groundLayerMask;
        protected int numRaycastHits = 0;

        protected readonly bool debug = true;

        public SurfaceCollisionEntity(GameObject gameObject, PhysicsEntity physicsEntity, CollisionWrapper collisionWrapper, float groundCheckRaycastDistance, float groundCheckRaycastSpread,
             float groundCheckCenterWeight, float groundCheckRaycastYOffset, LayerMask groundLayerMask, float slidingYAngleCutoff, float groundedYAngleCutoff,
             bool canSlide = true, bool useGroupRaycastNormals = true, bool useCollisionNormals = false)
        {
            this.gameObject = gameObject;
            this.physicsEntity = physicsEntity;
            this.collisionWrapper = collisionWrapper;
            this.groundCheckRaycastDistance = groundCheckRaycastDistance;
            this.groundCheckRaycastSpread = groundCheckRaycastSpread;
            this.groundCheckCenterWeight = groundCheckCenterWeight;
            this.groundCheckRaycastYOffset = groundCheckRaycastYOffset;
            this.groundLayerMask = groundLayerMask;
            this.slidingYAngleCutoff = slidingYAngleCutoff;
            this.groundedYAngleCutoff = groundedYAngleCutoff;
            this.canSlide = canSlide;
            this.useGroupRaycastNormals = useGroupRaycastNormals;
            this.useCollisionNormals = useCollisionNormals;

            if (useCollisionNormals == true)
            {
                this.collisionWrapper.AssignFunctionToCollisionEnterDelegate(SetGroundedFromCollision);
                this.collisionWrapper.AssignFunctionToCollisionStayDelegate(SetGroundedFromCollision);
            }
        }

        public void OnFixedUpdate()
        {
            isGrounded = false;
            isSliding = false;
            slopeNormalDotProduct = 0;
            if (useGroupRaycastNormals == true)
            {
                GetAveragedNormalFromDownwardRaycast(gameObject.transform.position, groundCheckRaycastDistance);
                if (numRaycastHits != 0)
                {
                    SetGroundedFromNormal(averagedHitNormal);
                }
            }
        }

        public Vector3 GetAveragedNormalFromDownwardRaycast(Vector3 position, float raycastDistance)
        {
            averagedHitNormal = Vector3.zero;
            numRaycastHits = 0;
            averagedHitNormal = GetNormalFromRaycast(position, 0f, 0f, raycastDistance) * groundCheckCenterWeight +
            GetNormalFromRaycast(position, groundCheckRaycastSpread, 0f, raycastDistance) +
            GetNormalFromRaycast(position, -groundCheckRaycastSpread, 0f, raycastDistance) +
            GetNormalFromRaycast(position, 0f, groundCheckRaycastSpread, raycastDistance) +
            GetNormalFromRaycast(position, 0f, -groundCheckRaycastSpread, raycastDistance);
            if (numRaycastHits != 0)
            {
                averagedHitNormal = averagedHitNormal / numRaycastHits;
            }
            return averagedHitNormal;
        }

        protected Vector3 GetNormalFromRaycast(Vector3 origin, float xOffset, float zOffset, float distance)
        {
            raycastOrigin = new Vector3(origin.x + xOffset, origin.y + groundCheckRaycastYOffset, origin.z + zOffset);
            Debug.DrawRay(raycastOrigin, Vector3.down * (distance + groundCheckRaycastYOffset), Color.magenta);
            if (Physics.Raycast(raycastOrigin, -Vector3.up, out hit, distance + groundCheckRaycastYOffset, groundLayerMask))
            {
                hitNormal = hit.normal;
                //Only pay attention to collisions that 
                if (Vector3.Angle(hitNormal, Vector3.up) <= slidingYAngleCutoff)
                {
                    numRaycastHits++;
                    return hit.normal;
                }
            }
            return Vector3.zero;
        }

        protected void SetGroundedFromCollision(Collision collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                SetGroundedFromNormal(collision.GetContact(i).normal);
            }
        }

        protected void SetGroundedFromNormal(Vector3 normal)
        {
            steepestSlopeYAngle = 0;
            steepestSlopeNormal = Vector3.zero;

            Vector3 normal2D = normal;
            normal2D.y = 0;

            //Get a vector that travels sideways along the collision plane. Has a y value of 0.
            Vector3 normalPerpendicularSide = Vector3.Cross(normal, normal2D).normalized;
            //Use the collision normal and the sideways perpendicular vector to get a new perpendicular vector that follows the slope of our collision plane.
            Vector3 normalPerpendicular = Vector3.Cross(normalPerpendicularSide, normal).normalized;
            //For determing the slope the entity is standing on, compare their contact normals to the up angle.
            float slopeDownAngle = Vector3.Angle(normal, Vector3.up);

            //If the entity is colliding with something that has a contact normal y angle less than groundedYAngleCutoff, we consider them grounded.
            isGrounded |= slopeDownAngle <= groundedYAngleCutoff;
            //If the entity is colliding with something that has a contact normal y angle less than slopeDownAngle but greater than groundedYAngleCutoff, we consider them sliding.
            isSliding |= slopeDownAngle > groundedYAngleCutoff && slopeDownAngle <= slidingYAngleCutoff;

            //Use the entity's input direction for calculating the slope normal dot product if they are moving. Otherwise, use their transform.forward.
            Vector3 entityDirection = gameObject.transform.forward;
            if (physicsEntity.desiredVelocity != Vector3.zero)
            {
                entityDirection = physicsEntity.desiredVelocity;
            }

            //If this slope is steeper than our last one without angling further downwards than sideways, then it is our new steepest slope.
            if (slopeDownAngle > steepestSlopeYAngle && slopeDownAngle < slidingYAngleCutoff)
            {
                steepestSlopeYAngle = slopeDownAngle;
                steepestSlopeNormal = normal;

                slopeNormalDotProduct = Vector3.Dot(entityDirection, steepestSlopeNormal);
                steepestSlopeNormalPerpendicularSide = normalPerpendicularSide;
                steepestSlopeNormalPerpendicular = normalPerpendicular;

                slopeNormal2D = steepestSlopeNormal;
                slopeNormal2D.y = 0f;
                slopeNormal2D.Normalize();
            }

            if (debug && (isGrounded || isSliding))
            {
                Debug.DrawRay(gameObject.transform.position, steepestSlopeNormal, Color.yellow);
                Debug.DrawRay(gameObject.transform.position, steepestSlopeNormalPerpendicularSide, Color.blue);
                Debug.DrawRay(gameObject.transform.position, steepestSlopeNormalPerpendicular, Color.green);
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