namespace GameAI.AIGameObjects
{
    using UnityEngine;
    using GameAI.Navigation;
    using GamePhysics;

    public class AIPhysics
    {
        private AIGameObjectData data;

        private PhysicsEntity physicsEntity;

        //Additional movement forces that make an agent attempt to keep away from other agents and obstacles.
        public Vector3 collisionAvoidanceForce = Vector3.zero;
        public Vector3 obstacleAvoidanceForce = Vector3.zero;

        //Adjusted avoidance forces that are decreased as the angle between them and the movement direction decreases.
        public Vector3 adjustedCollisionAvoidanceForce = Vector3.zero;
        public Vector3 adjustedObstacleAvoidanceForce = Vector3.zero;

        //Private variables used in the AdjustAvoidanceForceBasedOnMovementVelocity to store intermediate results.
        private Vector3 adjustedForce;
        private Vector3 difference;

        private bool alwaysFaceTarget = false;

        public void Init(AIGameObjectData data)
        {
            this.data = data;
            physicsEntity = new PhysicsEntity(data.gameObject, data.rb, data.capsuleCollider.center, data.capsuleCollider.height, data.capsuleCollider.radius);
        }

        public void ResetDesiredVelocity()
        {
            physicsEntity.ResetDesiredVelocity();
        }

        public virtual void CalculateVelocity(Vector3 velocity, bool ignoreYValue = true, float speedModifier = 1.0f, bool alwaysFaceTarget = false)
        {
            this.alwaysFaceTarget = alwaysFaceTarget;
            physicsEntity.CalculateVelocity(velocity.normalized, data.aiStats.speed, float.MaxValue, ignoreYValue);

            adjustedCollisionAvoidanceForce = AdjustAvoidanceForceBasedOnMovementVelocity(collisionAvoidanceForce, physicsEntity.velocity);
            adjustedObstacleAvoidanceForce = AdjustAvoidanceForceBasedOnMovementVelocity(obstacleAvoidanceForce, physicsEntity.velocity);

            physicsEntity.AddForceToVelocity(adjustedCollisionAvoidanceForce);
            physicsEntity.AddForceToVelocity(adjustedObstacleAvoidanceForce);

            if (data.debugFlocking)
            {
                Debug.Log("VELOCITY FORCE: " + physicsEntity.velocity);
                Debug.Log("ADJUSTED COLLISION AVOIDANCE FORCE: " + adjustedCollisionAvoidanceForce.magnitude);
                Debug.Log("ADJUSTED OBSTACLE AVOIDANCE FORCE: " + adjustedObstacleAvoidanceForce.magnitude);
            }

            physicsEntity.ApplyVelocityModifier(speedModifier);
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
            physicsEntity.LaunchEntity(direction, yForce, launchSpeed, rotationSpeed);
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
                physicsEntity.RotateEntity(data.aiStats.rotateSpeed);
            }
            physicsEntity.ApplyVelocity();
        }

        public virtual void ApplyGravity()
        {
            // Apply a force directly so we can handle gravity on our own instead of relying on rigidbody gravity.
            //TODO: Make stuff to tell whether or not enemies are grounded.
            physicsEntity.ApplyGravity(data.aiStats.gravity, data.aiStats.speed, false, 0);
        }

        public virtual void ResetVelocity()
        {
            physicsEntity.ApplyStationaryVelocity();
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

        public virtual void Rotate(float turningSpeed, bool stationaryTurn = false, Vector3? directionOverride = null)
        {
            if (alwaysFaceTarget)
            {
                physicsEntity.RotateEntity(turningSpeed, stationaryTurn, data.aggroTarget.transform.position - data.gameObject.transform.position);
                alwaysFaceTarget = false;
            }
            else
            {
                physicsEntity.RotateEntity(turningSpeed, stationaryTurn, directionOverride);
            }
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

        public Vector3 GetTransformForward()
        {
            return data.gameObject.transform.forward;
        }

        public Vector3 GetVelocity()
        {
            return physicsEntity.velocity;
        }

        public PhysicsEntity GetPhysicsEntity()
        {
            return physicsEntity;
        }
    }
}
