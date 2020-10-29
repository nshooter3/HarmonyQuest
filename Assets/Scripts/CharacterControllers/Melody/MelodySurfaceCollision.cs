namespace Melody
{
    using GamePhysics;
    using UnityEngine;

    public class MelodySurfaceCollision
    {
        //Surface collision for where Melody is currently standing.
        private SurfaceCollisionEntity surfaceCollisionEntity;
        //Surface collision for where Melody is attempting to stand before her velocity is applied.
        //Used to check for and prevent Melody from walking onto steep slopes without changing her actual grounded data.
        private SurfaceCollisionEntity preemptiveSurfaceCollisionEntity;

        public MelodySurfaceCollision(MelodyController controller)
        {
            surfaceCollisionEntity = new SurfaceCollisionEntity(controller.gameObject, controller.melodyPhysics.GetPhysicsEntity(), controller.melodyColliderWrapper, controller.config.groundCheckRaycastDistance,
                controller.config.groundCheckRaycastSpread, controller.config.groundCheckCenterWeight, controller.config.groundCheckRaycastYOffset, controller.config.groundLayerMask,
                controller.config.slidingYAngleCutoff, controller.config.groundedYAngleCutoff, true, true, false);

            preemptiveSurfaceCollisionEntity = new SurfaceCollisionEntity(controller.gameObject, controller.melodyPhysics.GetPhysicsEntity(), controller.melodyColliderWrapper, controller.config.groundCheckRaycastDistance,
                controller.config.groundCheckRaycastSpread, controller.config.groundCheckCenterWeight, controller.config.groundCheckRaycastYOffset, controller.config.groundLayerMask,
                controller.config.slidingYAngleCutoff, controller.config.groundedYAngleCutoff, true, true, false);
        }

        public void OnFixedUpdate()
        {
            surfaceCollisionEntity.OnFixedUpdate();
        }

        public bool IsGrounded()
        {
            return surfaceCollisionEntity.IsGrounded();
        }

        public bool IsSliding()
        {
            return surfaceCollisionEntity.IsSliding();
        }

        public bool IsInAir()
        {
            return surfaceCollisionEntity.IsInAir();
        }

        public float GetSlopeNormalDotProduct()
        {
            return surfaceCollisionEntity.slopeNormalDotProduct;
        }

        public bool IsMovementDestinationASteepSlope(Vector3 position, float raycastDistance)
        {
            return preemptiveSurfaceCollisionEntity.IsMovementDestinationASteepSlope(position, raycastDistance);
        }

        public SurfaceCollisionEntity GetPreemptiveSurfaceCollisionEntity()
        {
            return preemptiveSurfaceCollisionEntity;
        }
    }
}
