namespace GamePhysics
{
    using UnityEngine;

    public class PreemptiveSurfaceCollisionEntity : SurfaceCollisionEntity
    {
        public PreemptiveSurfaceCollisionEntity(GameObject gameObject, PhysicsEntity physicsEntity, CollisionWrapper collisionWrapper, float groundCheckRaycastDistance, float groundCheckRaycastSpread,
             float groundCheckCenterWeight, float groundCheckRaycastYOffset, LayerMask groundLayerMask, float slidingYAngleCutoff, float groundedYAngleCutoff,
             bool canSlide = true, bool useGroupRaycastNormals = true, bool useCollisionNormals = false) :

             base(gameObject, physicsEntity, collisionWrapper, groundCheckRaycastDistance, groundCheckRaycastSpread,
             groundCheckCenterWeight, groundCheckRaycastYOffset, groundLayerMask, slidingYAngleCutoff, groundedYAngleCutoff,
             canSlide = true, useGroupRaycastNormals = true, useCollisionNormals = false)
        { }

        //Simulate whether or not the player will standing on a steep slope before they've actually moved, so that we can prevent this movement.
        //Will be overwritten with actual slope information once OnFixedUpdate runs.
        public bool IsMovementDestinationASteepSlope(Vector3 position, float raycastDistance)
        {
            isSliding = false;
            GetAveragedNormalFromDownwardRaycast(position, raycastDistance);
            SetGroundedFromNormal(averagedHitNormal);
            return isSliding;
        }
    }
}