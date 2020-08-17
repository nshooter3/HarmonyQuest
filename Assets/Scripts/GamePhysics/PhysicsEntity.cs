namespace GamePhysics
{
    using UnityEngine;

    public class PhysicsEntity
    {
        public Vector3 velocity = Vector3.zero;
        public Vector3 desiredVelocity = Vector3.zero;
        public Vector3 acceleration = Vector3.zero;
        public float maxSpeedChange;

        public GameObject gameObject;
        public Rigidbody rb;

        //Create three offsets from the entity's pivot point for generating an upper, central, and lower point to raycast from when checking for wall collisions.
        public Vector3 colliderOffset;
        public Vector3 upperColliderOffset;
        public Vector3 lowerColliderOffset;

        //Raycast origin points
        public Vector3 colliderCenterPosition;
        public Vector3 colliderUpperPosition;
        public Vector3 colliderLowerPosition;

        //The radius of the entity's collider, added to her velocity when determing the distance to check for wall collisions.
        private float colliderRadius;
        //Used to determine how far from the center of our collider that the upper and lower raycasts should be. 0 for no separation, 1 for the very top and bottom of the entity's collider.
        private float upperLowerYHeightScale;

        //How far the entity is about to travel based on velocity
        public float predictedMovementDistance;

        private RaycastHit hit;

        private bool debug = false;

        public PhysicsEntity(GameObject gameObject, Rigidbody rb, Vector3 colliderOffset, float colliderHeight, float colliderRadius, float upperLowerYHeightScale = 0.5f)
        {
            this.gameObject = gameObject;
            this.rb = rb;
            this.colliderOffset = colliderOffset;
            this.colliderRadius = colliderRadius;
            this.upperLowerYHeightScale = upperLowerYHeightScale;
            upperColliderOffset = colliderOffset;
            upperColliderOffset.y += (colliderHeight / 2.0f) * upperLowerYHeightScale;
            lowerColliderOffset = colliderOffset;
            lowerColliderOffset.y += (-colliderHeight / 2.0f) * upperLowerYHeightScale;
        }

        public void ResetDesiredVelocity()
        {
            desiredVelocity = Vector3.zero;
        }

        public void OverrideVelocity(Vector3 newVelocity)
        {
            velocity = newVelocity;
        }

        public void CalculateVelocity(Vector3 direction, float maxSpeed, float maxAcceleration, bool ignoreYValue = true)
        {
            desiredVelocity = new Vector3(direction.x, 0, direction.z) * maxSpeed;
            maxSpeedChange = maxAcceleration * Time.deltaTime;
            velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
            velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
            if (ignoreYValue)
            {
                //Keep whatever our rigidbody y velocity was on the last frame to ensure that gravity works properly.
                velocity.y = rb.velocity.y;
            }
            else
            {
                velocity.y = Mathf.MoveTowards(velocity.y, desiredVelocity.y, maxSpeedChange);
            }
        }

        public void AddForceToVelocity(Vector3 force)
        {
            velocity = velocity + force;
        }

        public void ApplyVelocityModifier(float modifier)
        {
            desiredVelocity = desiredVelocity * modifier;
            velocity = velocity * modifier;
        }

        public void ApplyVelocity()
        {
            rb.velocity = velocity;
        }

        /// <summary>
        /// One frame function that suddenly launches the entity in a particular direction.
        /// Intended to juice up enemy deaths and such.
        /// </summary>
        /// <param name="direction"> Which direction the entity goes flying in. </param>
        /// <param name="yForce"> How high up the entity gets launched. </param>
        /// <param name="launchSpeed"> How quickly the entity is launched. </param>
        /// <param name="rotationSpeed"> How quickly the entity spins around in the air. </param>
        public void LaunchEntity(Vector3 direction, float yForce, float launchSpeed, float rotationSpeed)
        {
            desiredVelocity = direction.normalized;
            desiredVelocity = new Vector3(velocity.x, yForce, velocity.z);

            rb.angularVelocity = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized * rotationSpeed;

            velocity = desiredVelocity * launchSpeed;
        }

        public void ApplyGravity(Vector3 gravity, float maxSpeed, bool isGrounded, float slopeNormalDotProduct, bool isIdle = false)
        {
            if (isGrounded == false || (isIdle == false && slopeNormalDotProduct > 0.1f))
            {
                //Apply gravity if agent is in the air or sliding, or if they are moving downhill.
                rb.AddForce(gravity, ForceMode.Acceleration);
            }
            else
            {
                //Don't apply gravity if we are grounded, as this can sometimes lead to sliding when the agent stands on slight slopes.
                rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
            }

            //Cap speed after applying gravity when grounded to prevent the entity from moving too quickly downhill.
            if (isGrounded == true)
            {
                CapSpeed(maxSpeed);
            }
        }

        public void RotateEntity(float turningSpeed, bool stationaryTurn = false, Vector3? directionOverride = null)
        {
            if (desiredVelocity.magnitude > 0.0f || stationaryTurn == true)
            {
                Vector3 targetDir = desiredVelocity;

                if (directionOverride != null)
                {
                    targetDir = directionOverride.Value;
                }

                float step = turningSpeed * Time.deltaTime;

                Vector3 newDir = Vector3.RotateTowards(gameObject.transform.forward, targetDir, step, 0.0f);
                if (debug)
                {
                    Debug.DrawRay(gameObject.transform.position, newDir, Color.red);
                }

                // Move our position a step closer to the target.
                gameObject.transform.rotation = Quaternion.LookRotation(newDir);
            }
            //Failsafe to ensure that x and z are always zero.
            gameObject.transform.eulerAngles = new Vector3(0.0f, gameObject.transform.eulerAngles.y, 0.0f);
        }

        public void InstantFaceDirection(Vector3 direction)
        {
            RotateEntity(float.MaxValue, true, direction);
        }

        public void CapSpeed(float maxSpeed)
        {
            if (velocity.magnitude > maxSpeed)
            {
                velocity = velocity.normalized * maxSpeed;
            }
        }

        private void SetRaycastOriginPoints()
        {
            //Calculate the approximate distance that will be traversed, accounting for the radius of our collider.
            predictedMovementDistance = velocity.magnitude * Time.deltaTime + colliderRadius;

            //Raycast from the top, center, and bottom of the entity's collider to check for potential collisions.
            colliderCenterPosition = gameObject.transform.position + colliderOffset;
            colliderUpperPosition = gameObject.transform.position + upperColliderOffset;
            colliderLowerPosition = gameObject.transform.position + lowerColliderOffset;
        }

        /// <summary>
        /// Used to prevent the entity from walking into walls and halting their descent during a fall.
        /// We shoot three raycasts out from various heights on the entity, using her velocity and collider radius to predict where she will be on the next frame.
        /// If any of these raycast hit an object in layerMask, cancel all horizontal movement.
        /// </summary>
        public void ProhibitMovementIntoWalls(LayerMask layerMask, bool isDash = false)
        {
            SetRaycastOriginPoints();

            //Check if the body's current velocity will result in a collision
            if (Physics.Raycast(colliderCenterPosition, velocity.normalized, out hit, predictedMovementDistance, layerMask) ||
                Physics.Raycast(colliderUpperPosition, velocity.normalized, out hit, predictedMovementDistance, layerMask) ||
                (Physics.Raycast(colliderLowerPosition, velocity.normalized, out hit, predictedMovementDistance, layerMask) && !isDash))
            {
                if (isDash == true)
                {
                    //If the entity dashes into a wall, cancel their movement for the remainder of the dash.
                    velocity = Vector3.zero;
                }
                else
                {
                    //If the entity walks into a wall, stop the horizontal movement
                    IgnoreHorizontalMovementInput();
                }
            }
            if (debug)
            {
                Debug.DrawRay(colliderCenterPosition, velocity.normalized * predictedMovementDistance, Color.yellow);
                Debug.DrawRay(colliderUpperPosition, velocity.normalized * predictedMovementDistance, Color.blue);
                Debug.DrawRay(colliderLowerPosition, velocity.normalized * predictedMovementDistance, Color.green);
            }
        }

        public void ApplyStationaryVelocity()
        {
            velocity = Vector3.zero;
            rb.velocity = velocity;
        }

        public void IgnoreHorizontalMovementInput()
        {
            velocity = new Vector3(0.0f, velocity.y, 0.0f);
        }

        public void SnapToGround(bool isGrounded, float snapToGroundRaycastDistance, LayerMask layerMask)
        {
            if (!isGrounded)
            {
                if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, snapToGroundRaycastDistance, layerMask))
                {
                    rb.MovePosition(hit.point);
                }
            }
        }
    }
}
