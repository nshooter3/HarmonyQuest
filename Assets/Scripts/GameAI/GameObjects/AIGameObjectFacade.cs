namespace GameAI.AIGameObjects
{
    using Navigation;
    using UnityEngine;
    using GameAI.AIStates;

    public abstract class AIGameObjectFacade : MonoBehaviour
    {
        [SerializeField]
        public AIGameObjectData data;

        private AIPhysics aiPhysics = new AIPhysics();
        private AICollision aiCollision = new AICollision();
        private AIDebug aiDebug = new AIDebug();

        // ****************************
        // CHILD OVERRIDE FUNCTIONS
        // ****************************

        /// <summary>
        /// Implement this in the child class to specify which state the enemy will start in.
        /// </summary>
        /// <returns> A new instance of this agent's initial state </returns>
        public abstract AIState GetInitState();

        /// <summary>
        /// Implement this in the child class to specify what kind of navigator this agent will use.
        /// </summary>
        /// <returns> A new instance of this agent's navigator </returns>
        public abstract Navigator GetNavigator();

        public virtual void Init()
        {
            if (data.gameObject == null)
            {
                data.gameObject = gameObject;
            }

            if (data.rb == null)
            {
                data.rb = GetComponent<Rigidbody>();
            }

            if (data.strafeHitBoxes != null)
            {
                data.strafeHitBoxes.Init();
            }

            data.origin.parent = null;
            if (NavMeshUtil.IsNavMeshBelowTransform(transform, out Vector3 navmeshPosBelowOrigin))
            {
                data.origin.transform.position = navmeshPosBelowOrigin;
            }
            else
            {
                Debug.LogError("AIGameObject Init WARNING: Agent origin not located on or above navmesh.");
            }

            data.navPos.transform.parent = null;
            data.navPos.SetActive(data.showDestination);
            data.defaultConstraints = data.rb.constraints;

            data.aggroTarget = TestPlayer.instance.transform;

            aiPhysics.Init(data);
            aiCollision.Init(data);
            aiDebug.Init(data);
        }

        // ****************************
        // PHYSICS FUNCTIONS
        // ****************************

        public virtual void SetVelocityTowardsDestination(Vector3 destination, bool ignoreYValue = true, float speedModifier = 1.0f, bool alwaysFaceTarget = false)
        {
            SetVelocity((destination - data.aiAgentBottom.position).normalized, ignoreYValue, speedModifier, alwaysFaceTarget);
        }

        public virtual void SetVelocityAwayFromDestination(Vector3 destination, bool ignoreYValue = true, float speedModifier = 1.0f, bool alwaysFaceTarget = false)
        {
            SetVelocity((data.aiAgentBottom.position - destination).normalized, ignoreYValue, speedModifier, alwaysFaceTarget);
        }

        public virtual void SetVelocity(Vector3 velocity, bool ignoreYValue = true, float speedModifier = 1.0f, bool alwaysFaceTarget = false)
        {
            aiPhysics.SetVelocity(velocity, ignoreYValue, speedModifier, alwaysFaceTarget);
        }

        public virtual void ApplyVelocity(bool ignoreYValue = true)
        {
            aiPhysics.ApplyVelocity(ignoreYValue);
        }

        public virtual void ApplyGravity()
        {
            aiPhysics.ApplyGravity();
        }

        public virtual void ResetVelocity()
        {
            aiPhysics.ResetVelocity();
        }

        public virtual Vector3 GetCollisionAvoidanceForce()
        {
            return aiPhysics.GetCollisionAvoidanceForce();
        }

        public virtual void SetCollisionAvoidanceForce(Vector3 collisionAvoidanceForce)
        {
            aiPhysics.SetCollisionAvoidanceForce(collisionAvoidanceForce);
        }

        public virtual void SetObstacleAvoidanceForce(Vector3 obstacleAvoidanceForce)
        {
            aiPhysics.SetObstacleAvoidanceForce(obstacleAvoidanceForce);
        }

        public virtual void Rotate(Vector3 direction, float turnSpeedModifier)
        {
            aiPhysics.Rotate(direction, turnSpeedModifier);
        }

        public virtual void SetRigidbodyConstraints(RigidbodyConstraints constraints)
        {
            aiPhysics.SetRigidbodyConstraints(constraints);
        }

        public virtual void SetRigidBodyConstraintsToDefault()
        {
            aiPhysics.SetRigidBodyConstraintsToDefault();
        }

        public virtual void SetRigidBodyConstraintsToLockAllButGravity()
        {
            aiPhysics.SetRigidBodyConstraintsToLockAllButGravity();
        }

        public virtual void SetRigidBodyConstraintsToLockMovement()
        {
            aiPhysics.SetRigidBodyConstraintsToLockMovement();

        }

        public virtual float GetDistanceFromAggroTarget()
        {
            return aiPhysics.GetDistanceFromAggroTarget();
        }

        // ****************************
        // COLLISION FUNCTIONS
        // ****************************
        public virtual Collider[] GetHurtboxes()
        {
            return aiCollision.GetHurtboxes();
        }

        public virtual Collider GetCollisionAvoidanceHitbox()
        {
            return aiCollision.GetCollisionAvoidanceHitbox();
        }

        // ****************************
        // DEBUG FUNCTIONS
        // ****************************

        public virtual void DebugChangeColor(Color color)
        {
            aiDebug.DebugChangeColor(color);
        }
    }
}
