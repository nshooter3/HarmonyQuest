namespace Melody
{
    using UnityEngine;

    public class MelodyPhysics
    {
        private MelodyController controller;

        public Vector3 velocity = Vector3.zero;
        private Vector3 desiredVelocity = Vector3.zero;
        private Vector3 acceleration = Vector3.zero;
        private float maxSpeedChange;

        private Vector3 slideVelocity;

        //Create three offsets from Melody's pivot point for generating an upper, central, and lower point to raycast from when checking for wall collisions.
        private Vector3 colliderOffset;
        private Vector3 upperColliderOffset;
        private Vector3 lowerColliderOffset;
        //The radius of Melody's collider, added to her velocity when determing the distance to check for wall collisions.
        private float colliderRadius;
        //Used to determine how far from the center of our collider that the upper and lower raycasts should be. 0 for no separation, 1 for the very top and bottom of Melody's collider.
        private float upperLowerYHeightScale = 0.5f;

        private RaycastHit hit;

        public MelodyPhysics(MelodyController controller)
        {
            this.controller = controller;
            colliderOffset = controller.capsuleCollider.center;
            upperColliderOffset = colliderOffset;
            upperColliderOffset.y += (controller.capsuleCollider.height / 2.0f) * upperLowerYHeightScale;
            lowerColliderOffset = colliderOffset;
            lowerColliderOffset.y += (-controller.capsuleCollider.height / 2.0f) * upperLowerYHeightScale;
            colliderRadius = controller.capsuleCollider.radius;
        }

        public void CalculateVelocity(float maxSpeed, float maxAcceleration)
        {
            desiredVelocity = new Vector3(controller.move.x, 0, controller.move.z) * maxSpeed;
            maxSpeedChange = maxAcceleration * Time.deltaTime;
            velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
            velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
            //Keep whatever our rigidbody y velocity was on the last frame to ensure that gravity works properly.
            velocity.y = controller.rigidBody.velocity.y;
        }

        public void ApplyVelocity(float maxSpeed, float turningSpeed)
        {
            if (controller.melodyCollision.IsSliding())
            {
                SetVelocityToSlide();
            }
            else
            {
                ProhibitMovementIntoWalls();
            }
            RotatePlayer(turningSpeed);
            controller.rigidBody.velocity = velocity;
            controller.melodyAnimator.SetWalkRun(desiredVelocity.magnitude / maxSpeed);

        }

        public void ApplyDashVelocity(Vector3 dashVelocity)
        {
            velocity = dashVelocity;
            ProhibitMovementIntoWalls(true);
            controller.rigidBody.velocity = velocity;
        }

        public void CapSpeed(float maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }

        /// <summary>
        /// Used to prevent the player from walking into walls and halting their descent during a fall.
        /// We shoot three raycasts out from various heights on Melody, using her velocity and collider radius to predict where she will be on the next frame.
        /// If any of these raycast hit an object in prohibitMovementIntoWallsLayerMask, cancel all horizontal movement.
        /// </summary>
        private void ProhibitMovementIntoWalls(bool isDash = false)
        {
            //Calculate the approximate distance that will be traversed, accounting for the radius of our collider.
            float distance = velocity.magnitude * Time.deltaTime + colliderRadius;
            //Raycast from the top, center, and bottom of the player's collider to check for potential collisions.
            Vector3 colliderCenterPosition = controller.transform.position + colliderOffset;
            Vector3 colliderUpperPosition = controller.transform.position + upperColliderOffset;
            Vector3 colliderLowerPosition = controller.transform.position + lowerColliderOffset;
            //Check if the body's current velocity will result in a collision
            if (Physics.Raycast(colliderCenterPosition, velocity.normalized, out hit, distance, controller.config.prohibitMovementIntoWallsLayerMask) ||
                Physics.Raycast(colliderUpperPosition,  velocity.normalized, out hit, distance, controller.config.prohibitMovementIntoWallsLayerMask) ||
                Physics.Raycast(colliderLowerPosition,  velocity.normalized, out hit, distance, controller.config.prohibitMovementIntoWallsLayerMask) )
            {
                if (isDash)
                {
                    //If the player dashes into a wall, cancel their movement for the remainder of the dash.
                    velocity = Vector3.zero;
                }
                else
                {
                    //If the player walks into a wall, stop the horizontal movement
                    IgnoreHorizontalMovementInput();
                }
            }
            Debug.DrawRay(colliderCenterPosition, velocity.normalized * distance, Color.yellow);
            Debug.DrawRay(colliderUpperPosition, velocity.normalized * distance, Color.blue);
            Debug.DrawRay(colliderLowerPosition, velocity.normalized * distance, Color.green);
        }

        private void SetVelocityToSlide()
        {
            slideVelocity = new Vector3(controller.melodyCollision.steepestSlopeDirection.x * controller.config.slidingSpeedAdjusmentRatio.x,
                                              velocity.y * controller.config.slidingSpeedAdjusmentRatio.y,
                                              controller.melodyCollision.steepestSlopeDirection.z * controller.config.slidingSpeedAdjusmentRatio.z);

            if (controller.input.GetHorizontalMovement() != 0 || controller.input.GetVerticalMovement() != 0)
            {
                //Give the player some influence over their movement while sliding if they try to move
                velocity.x = Mathf.Lerp(slideVelocity.x, velocity.x, controller.config.slidingControllerInfluenceRatio);
                velocity.y = slideVelocity.y;
                velocity.z = Mathf.Lerp(slideVelocity.z, velocity.z, controller.config.slidingControllerInfluenceRatio);
            }
            else
            {
                velocity = slideVelocity;
            }
            Debug.DrawRay(controller.transform.position + colliderOffset, velocity * 2.0f, Color.cyan);
        }

        private void IgnoreHorizontalMovementInput()
        {
            velocity = new Vector3(0.0f, velocity.y, 0.0f);
        }

        public void ApplyGravity(Vector3 gravity)
        {
            if (controller.melodyCollision.IsGrounded() == false)
            {
                //Apply gravity if Melody is in the air or sliding.
                controller.rigidBody.AddForce(gravity, ForceMode.Acceleration);
            }
            else
            {
                //Don't apply gravity if we are grounded, as this can sometimes lead to sliding when Melody stands on slight slopes.
                controller.rigidBody.velocity = new Vector3(controller.rigidBody.velocity.x, 0.0f, controller.rigidBody.velocity.z);
            }
        }

        public void SnapToGround()
        {
            if (controller.melodyCollision.IsInAir())
            {
                if (Physics.Raycast(controller.transform.position, Vector3.down, out hit, controller.config.snapToGroundRaycastDistance, controller.config.snapToGroundLayerMask))
                {
                    controller.transform.position = hit.point + controller.config.snapOffset;
                }
            }
        }

        public void RotatePlayer(float turningSpeed, bool stationaryTurn = false)
        {
            //Rotate player to face movement direction
            if (controller.move.magnitude > 0.0f || stationaryTurn == true)
            {
                Vector3 targetDir;

                if (controller.melodyLockOn.HasLockonTarget() == true)
                {
                    targetDir = controller.melodyLockOn.GetLockonTargetPosition() - controller.transform.position;
                }
                else
                {
                    targetDir = controller.move;
                }

                float step = controller.config.TurningSpeed * turningSpeed * Time.deltaTime;

                Vector3 newDir = Vector3.RotateTowards(controller.transform.forward, targetDir, step, 0.0f);
                Debug.DrawRay(controller.transform.position, newDir, Color.red);

                // Move our position a step closer to the target.
                controller.transform.rotation = Quaternion.LookRotation(newDir);
            }
            //Failsafe to ensure that x and z are always zero.
            controller.transform.eulerAngles = new Vector3(0.0f, controller.transform.eulerAngles.y, 0.0f);
        }
    }
}
