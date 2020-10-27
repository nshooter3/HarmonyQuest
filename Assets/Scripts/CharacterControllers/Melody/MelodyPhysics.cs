namespace Melody
{
    using UnityEngine;
    using Objects;
    using GamePhysics;

    public class MelodyPhysics
    {
        private MelodyController controller;
        private PhysicsEntity physicsEntity;

        //The box that Melody is currently pushing.
        PushableBox pushableBox;

        private RaycastHit hit;

        public MelodyPhysics(MelodyController controller)
        {
            this.controller = controller;
            physicsEntity = new PhysicsEntity(controller.gameObject, controller.rigidBody, controller.capsuleCollider.center, controller.capsuleCollider.height, controller.capsuleCollider.radius);
        }

        public void ResetDesiredVelocity()
        {
            physicsEntity.ResetDesiredVelocity();
        }

        public void CalculateVelocity(float maxSpeed, float maxAcceleration)
        {
            physicsEntity.CalculateVelocity(controller.move, maxSpeed, maxAcceleration);
        }

        public void ApplyVelocity(float maxSpeed, float turningSpeed, bool canPushBoxes = false)
        {
            if (controller.melodyCollision.IsSliding())
            {
                //Do nothing
            }
            else
            {
                physicsEntity.ProhibitMovementIntoWalls(controller.config.prohibitMovementIntoWallsLayerMask);
                if (canPushBoxes == true)
                {
                    PushBoxes();
                }
                RotatePlayer(turningSpeed);
                physicsEntity.ApplyVelocity();
            }
            controller.melodyAnimator.SetWalkRun(physicsEntity.desiredVelocity.magnitude / maxSpeed);
            controller.melodyAnimator.SetStrafeInfo(controller.transform.forward, physicsEntity.velocity);
        }

        public void ApplyDashVelocity(Vector3 dashVelocity)
        {
            if (controller.melodyCollision.IsSliding())
            {
                physicsEntity.ApplyStationaryVelocity();
            }
            else
            {
                physicsEntity.velocity = dashVelocity;
                physicsEntity.ProhibitMovementIntoWalls(controller.config.prohibitDashIntoWallsLayerMask, true);
                physicsEntity.ApplyVelocity();
            }
        }

        public void SetPosition(Vector3 position)
        {
            physicsEntity.SetPosition(position);
        }

        private void PushBoxes()
        {
            pushableBox = null;
            if (controller.melodyCollision.IsGrounded() == true)
            {
                //Check if the body's current velocity will result in a collision
                if (Physics.Raycast(physicsEntity.colliderCenterPosition, physicsEntity.velocity.normalized, out hit, physicsEntity.predictedMovementDistance, controller.config.boxPushingLayerMask) ||
                    Physics.Raycast(physicsEntity.colliderUpperPosition, physicsEntity.velocity.normalized, out hit, physicsEntity.predictedMovementDistance, controller.config.boxPushingLayerMask) ||
                    Physics.Raycast(physicsEntity.colliderLowerPosition, physicsEntity.velocity.normalized, out hit, physicsEntity.predictedMovementDistance, controller.config.boxPushingLayerMask))
                {
                    pushableBox = hit.transform.gameObject.GetComponent<PushableBox>();
                    if (pushableBox != null)
                    {
                        pushableBox.moveThisFrame = true;
                    }
                }
            }
        }

        public bool IsPushingBox()
        {
            return pushableBox != null;
        }

        public void ApplyGravity(Vector3 gravity, bool isIdle = false)
        {
            physicsEntity.ApplyGravity(gravity, controller.config.MaxSpeed, controller.melodyCollision.IsGrounded(), controller.melodyCollision.GetSlopeNormalDotProduct(), isIdle);
        }

        public void ApplyDashGravity(Vector3 gravity)
        {
            //Apply gravity if Melody is moving downhill.
            if (controller.melodyCollision.GetSlopeNormalDotProduct() > 0.1f)
            {
                controller.rigidBody.AddForce(gravity, ForceMode.VelocityChange);
            }

            //Cap speed after applying gravity when grounded to prevent Melody from moving too quickly downhill.
            if (controller.melodyCollision.IsGrounded() == true)
            {
                physicsEntity.CapSpeed(controller.config.DashLength / controller.config.DashTime);
            }
        }

        public Vector3 GetVelocity()
        {
            return physicsEntity.velocity;
        }

        public void RotatePlayer(float turningSpeed, bool stationaryTurn = false)
        {
            if (controller.HasLockonTarget())
            {
                physicsEntity.RotateEntity(turningSpeed, stationaryTurn, controller.GetLockonTarget().aiGameObject.transform.position - controller.transform.position);
            }
            else if (IsPushingBox() == true)
            {
                physicsEntity.RotateEntity(turningSpeed, stationaryTurn, pushableBox.GetBoxcastDirection());
            }
            else
            {
                physicsEntity.RotateEntity(turningSpeed, stationaryTurn);
            }
        }

        public void InstantFaceDirection(Vector3 direction)
        {
            physicsEntity.InstantFaceDirection(direction);
        }

        public void SnapToGround()
        {
            physicsEntity.SnapToGround(controller.melodyCollision.IsGrounded(), controller.config.snapToGroundRaycastDistance, controller.config.groundLayerMask);
        }

        public void OverrideVelocity(Vector3 newVelocity)
        {
            physicsEntity.OverrideVelocity(newVelocity);
        }

        public void CapSpeed(float maxSpeed)
        {
            physicsEntity.CapSpeed(maxSpeed);
        }

        public void ApplyStationaryVelocity()
        {
            physicsEntity.ApplyStationaryVelocity();
        }

        public void IgnoreHorizontalMovementInput()
        {
            physicsEntity.IgnoreHorizontalMovementInput();
        }

        public PhysicsEntity GetPhysicsEntity()
        {
            return physicsEntity;
        }

        public void ToggleIsKinematic(bool isKinematic)
        {
            physicsEntity.ToggleIsKinematic(isKinematic);
        }
    }
}
