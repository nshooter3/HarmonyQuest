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

        //Create three offsets from Melody's pivot point for generating an upper, central, and lower point to raycast from when checking for wall collisions.
        private Vector3 colliderOffset;
        private Vector3 upperColliderOffset;
        private Vector3 lowerColliderOffset;
        //The radius of Melody's collider, added to her velocity when determing the distance to check for wall collisions.
        private float colliderRadius;

        private RaycastHit hit;

        public MelodyPhysics(MelodyController controller)
        {
            this.controller = controller;
            colliderOffset = controller.capsuleCollider.center;
            upperColliderOffset = colliderOffset;
            upperColliderOffset.y = (controller.capsuleCollider.height / 2.0f) * 0.9f;
            lowerColliderOffset = colliderOffset;
            lowerColliderOffset.y = (-controller.capsuleCollider.height / 2.0f) * 0.9f;
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
            controller.animator.SetFloat("Move", desiredVelocity.magnitude / maxSpeed);
        }

        /// <summary>
        /// Used to prevent the player from walking into walls and halting their descent during a fall.
        /// We shoot three raycasts out from various heights on Melody, using her velocity and collider radius to predict where she will be on the next frame.
        /// If any of these raycast hit an object in prohibitMovementIntoWallsLayerMask, cancel all horizontal movement.
        /// </summary>
        private void ProhibitMovementIntoWalls()
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
                //If so, stop the horizontal movement
                IgnoreHorizontalMovementInput();
            }
            Debug.DrawRay(colliderCenterPosition, velocity.normalized, Color.blue);
        }

        private void SetVelocityToSlide()
        {
            //TODO: Any other custom behavior we want for sliding.
            IgnoreHorizontalMovementInput();
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

        public void RotatePlayer(float turningSpeed)
        {
            //Rotate player to face movement direction
            if (controller.move.magnitude > 0.0f)
            {
                Vector3 targetPos = controller.transform.position + controller.move;
                Vector3 targetDir = targetPos - controller.transform.position;

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
