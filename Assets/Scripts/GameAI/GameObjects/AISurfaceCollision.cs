namespace GameAI.AIGameObjects
{
    using GamePhysics;
    using HarmonyQuest;
    using Melody;

    public class AISurfaceCollision
    {
        private SurfaceCollisionEntity surfaceCollisionEntity;

        //Use Melody's collision params for now
        private MelodyController controller;

        public void Init()
        {
            //TODO: Make this actually use enemy params, ya dingus.
            controller = ServiceLocator.instance.GetMelodyController();
            surfaceCollisionEntity = new SurfaceCollisionEntity(controller.gameObject, controller.melodyPhysics.GetPhysicsEntity(), controller.melodyColliderWrapper, controller.config.groundCheckRaycastDistance,
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
    }
}
