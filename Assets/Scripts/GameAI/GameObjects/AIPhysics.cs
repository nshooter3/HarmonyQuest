namespace GameAI.AIGameObjects
{
    using UnityEngine;
    using GameAI.Navigation;

    public class AIPhysics
    {
        private AIGameObjectData data;

        public Vector3 moveDirection = Vector3.zero;
        public Vector3 rotationDirection = Vector3.zero;
        public Vector3 newVelocity = Vector3.zero;
        private Vector3 modifiedVelocity = Vector3.zero;
        private float prevYVel = 0;

        //Additional movement forces that make an agent attempt to keep away from other agents and obstacles.
        public Vector3 collisionAvoidanceForce = Vector3.zero;
        public Vector3 obstacleAvoidanceForce = Vector3.zero;

        //Adjusted avoidance forces that are decreased as the angle between them and the movement direction decreases.
        public Vector3 adjustedCollisionAvoidanceForce = Vector3.zero;
        public Vector3 adjustedObstacleAvoidanceForce = Vector3.zero;

        //Private variables used in the AdjustAvoidanceForceBasedOnMovementVelocity to store intermediate results.
        private Vector3 adjustedForce;
        private Vector3 difference;

        public void Init(AIGameObjectData data)
        {
            this.data = data;
        }

        public virtual void SetVelocity(Vector3 velocity, bool ignoreYValue = true, float speedModifier = 1.0f, bool alwaysFaceTarget = false)
        {
            moveDirection = velocity.normalized;

            SetRotationDirection(alwaysFaceTarget);

            if (ignoreYValue)
            {
                //Whatever the y value was before, keep it. This allows the AI to still fall off of things while ignoring y velocity on their movement vector.
                prevYVel = data.rb.velocity.y;
                newVelocity = (moveDirection * Time.deltaTime) * data.aiStats.speed;
                newVelocity.y = prevYVel;
            }
            else
            {
                newVelocity = (moveDirection * Time.deltaTime) * data.aiStats.speed;
            }

            adjustedCollisionAvoidanceForce = AdjustAvoidanceForceBasedOnMovementVelocity(collisionAvoidanceForce, newVelocity);
            adjustedObstacleAvoidanceForce = AdjustAvoidanceForceBasedOnMovementVelocity(obstacleAvoidanceForce, newVelocity);

            if (data.debugFlocking)
            {
                Debug.Log("VELOCITY FORCE: " + newVelocity.magnitude);
                Debug.Log("ADJUSTED COLLISION AVOIDANCE FORCE: " + adjustedCollisionAvoidanceForce.magnitude);
                Debug.Log("ADJUSTED OBSTACLE AVOIDANCE FORCE: " + adjustedObstacleAvoidanceForce.magnitude);
            }

            newVelocity = (newVelocity + adjustedCollisionAvoidanceForce + adjustedObstacleAvoidanceForce) * speedModifier;
        }

        public void SetRotationDirection(bool alwaysFaceTarget = false)
        {
            if (alwaysFaceTarget)
            {
                rotationDirection = (data.aggroTarget.transform.position - data.gameObject.transform.position).normalized;
            }
            else
            {
                rotationDirection = moveDirection;
            }
            rotationDirection.y = 0;
            rotationDirection.Normalize();
        }

        /// <summary>
        /// One frame function that suddenly launches the enemy in a particular direction.
        /// Intended to juice up enemy deaths and such.
        /// </summary>
        /// <param name="direction"> Which direction the enemy goes flying in. </param>
        /// <param name="yForce"> How high up the enemy gets launched. </param>
        /// <param name="launchSpeed"> How quickly the enemy is launched. </param>
        /// <param name="rotationSpeed"> How quickly the enemy spins around in the air. </param>
        public virtual void LaunchAgent(Vector3 direction, float yForce, float launchSpeed, float rotationSpeed)
        {
            moveDirection = direction.normalized;
            moveDirection = new Vector3(moveDirection.x, yForce, moveDirection.z);

            data.rb.angularVelocity = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized * rotationSpeed;

            newVelocity = moveDirection * launchSpeed;
        }

        /// <summary>
        /// Scale down avoidance forces as the angle between them and the movement direction decreases.
        /// This prevents redundant avoidance forces from speeding up the enemy rather than adjusting their path.
        /// </summary>
        /// <param name="avoidanceForce"></param>
        /// <param name="movementVelocity"></param>
        /// <returns></returns>
        private Vector3 AdjustAvoidanceForceBasedOnMovementVelocity(Vector3 avoidanceForce, Vector3 movementVelocity)
        {
            adjustedForce = avoidanceForce * (Vector3.Angle(movementVelocity, avoidanceForce) / 180.0f);
            difference = avoidanceForce - adjustedForce;
            return avoidanceForce - difference * NavigatorSettings.avoidanceForceMovementVelocityAdjustmentScale;
        }

        public virtual void ApplyVelocity(bool ignoreYValue = true, bool applyRotation = true, float turnSpeedModifier = 1.0f, bool instantlyFaceDirection = false)
        {
            if (applyRotation)
            {
                Rotate(rotationDirection, turnSpeedModifier, instantlyFaceDirection);
            }
            modifiedVelocity = newVelocity;
            if (ignoreYValue)
            {
                modifiedVelocity.y = 0;
            }
            data.rb.velocity = modifiedVelocity;
        }

        public virtual void ApplyGravity()
        {
            // Apply a force directly so we can handle gravity on our own instead of relying on rigidbody gravity.
            data.rb.AddForce(data.aiStats.gravity, ForceMode.Acceleration);
        }

        public virtual void ResetVelocity()
        {
            newVelocity = Vector3.zero;
        }

        public Vector3 GetCollisionAvoidanceForce()
        {
            return collisionAvoidanceForce;
        }

        public void SetCollisionAvoidanceForce(Vector3 collisionAvoidanceForce)
        {
            this.collisionAvoidanceForce = collisionAvoidanceForce;
        }

        public void SetObstacleAvoidanceForce(Vector3 obstacleAvoidanceForce)
        {
            this.obstacleAvoidanceForce = obstacleAvoidanceForce;
        }

        public virtual void Rotate(Vector3 direction, float turnSpeedModifier, bool instantlyFaceDirection = false)
        {
            //Rotate enemy to face movement direction
            if (direction.magnitude > 0)
            {
                Vector3 targetPos = data.gameObject.transform.position + direction;
                Vector3 targetDir = targetPos - data.gameObject.transform.position;

                // The step size is equal to speed times frame time.
                float step = data.aiStats.rotateSpeed * turnSpeedModifier * Time.deltaTime;

                Vector3 newDir;
                if (instantlyFaceDirection)
                {
                    newDir = targetDir;
                }
                else
                {
                    newDir = Vector3.RotateTowards(data.gameObject.transform.forward, targetDir, step, 0.0f);
                }
                Debug.DrawRay(data.gameObject.transform.position, newDir, Color.red);

                // Move our position a step closer to the target.
                data.gameObject.transform.rotation = Quaternion.LookRotation(newDir);
            }
            //Failsafe to ensure that x and z are always zero.
            data.gameObject.transform.eulerAngles = new Vector3(0, data.gameObject.transform.eulerAngles.y, 0);
        }

        public void SetRigidbodyConstraints(RigidbodyConstraints constraints)
        {
            data.rb.constraints = constraints;
        }

        public void SetRigidBodyConstraintsToDefault()
        {
            SetRigidbodyConstraints(data.defaultConstraints);
        }

        public void SetRigidBodyConstraintsToLockAllButGravity()
        {
            SetRigidbodyConstraints(RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ);
        }

        public void SetRigidBodyConstraintsToLockMovement()
        {
            SetRigidbodyConstraints(RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ |
                                    RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ);
        }

        public void SetRigidBodyConstraintsToNone()
        {
            SetRigidbodyConstraints(RigidbodyConstraints.None);
        }

        public float GetDistanceFromAggroTarget()
        {
            return Vector3.Distance(data.gameObject.transform.position, data.aggroTarget.transform.position);
        }

        public Vector3 GetMoveDirection()
        {
            return moveDirection;
        }

        public Vector3 GetRotationDirection()
        {
            return rotationDirection;
        }

        public Vector3 GetTransformForward()
        {
            return data.gameObject.transform.forward;
        }
    }
}
