namespace GameAI.AIGameObjects
{
    using Navigation;
    using UnityEngine;
    using GameAI.AIStates;
    using GamePhysics;
    using HarmonyQuest;

    public abstract class AIGameObjectFacade : MonoBehaviour
    {
        [SerializeField]
        public AIGameObjectData data;

        private AIPhysics aiPhysics = new AIPhysics();
        private AIHitboxes aiHitboxes = new AIHitboxes();
        private AIHealth aiHealth = new AIHealth();
        private AIDebug aiDebug = new AIDebug();
        private AIUtil aiUtil = new AIUtil();

        public bool requestingAttackPermission = false;
        public bool attackPermissionGranted = false;
        public bool attacking = false;

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

            data.aggroTarget = ServiceLocator.instance.GetMelodyInfo().GetTransform();

            aiPhysics.Init(data);
            aiHitboxes.Init(data);
            aiHealth.Init(data);
            aiDebug.Init(data);
            aiUtil.Init(data);

            DamageReceiver damageReceiver;

            foreach (Collider hurtbox in data.hurtboxes)
            {
                //Create and attach a DamageReceiver to all our hurtboxes at runtime
                damageReceiver = hurtbox.gameObject.AddComponent<DamageReceiver>();
                damageReceiver.AssignFunctionToReceiveDamageDelegate(aiHealth.ReceiveDamageHitbox);
            }
        }

        public void UpdateSubclasses()
        {
            if (IsDead() == false)
            {
                UpdateHitboxes();
                RemoveInactiveReceivedDamageHitboxes();
            }
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

        public virtual void ApplyVelocity(bool ignoreYValue = true, bool applyRotation = true, float turnSpeedModifier = 1.0f)
        {
            aiPhysics.ApplyVelocity(ignoreYValue, applyRotation, turnSpeedModifier);
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

        public virtual void SetRotationDirection(bool alwaysFaceTarget = false)
        {
            aiPhysics.SetRotationDirection(alwaysFaceTarget);
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

        public void SetRigidBodyConstraintsToNone()
        {
            aiPhysics.SetRigidBodyConstraintsToNone();
        }

        public virtual float GetDistanceFromAggroTarget()
        {
            return aiPhysics.GetDistanceFromAggroTarget();
        }

        public virtual void LaunchAgent(Vector3 direction, float yForce, float launchSpeed, float rotationSpeed)
        {
            aiPhysics.LaunchAgent(direction, yForce, launchSpeed, rotationSpeed);
        }

        public virtual Vector3 GetMoveDirection()
        {
            return aiPhysics.GetMoveDirection();
        }

        public virtual Vector3 GetRotationDirection()
        {
            return aiPhysics.GetRotationDirection();
        }

        // ****************************
        // COLLISION FUNCTIONS
        // ****************************

        public virtual Collider GetCollisionAvoidanceHitbox()
        {
            return aiHitboxes.GetCollisionAvoidanceHitbox();
        }

        public virtual void ActivateHitbox(string name, float delay, float lifetime, int damage)
        {
            aiHitboxes.ActivateHitbox(name, delay, lifetime, damage);
        }

        public virtual void CancelHitbox(string name)
        {
            aiHitboxes.CancelHitbox(name);
        }

        public virtual void CancelAllHitboxes()
        {
            aiHitboxes.CancelAllHitboxes();
        }

        private void UpdateHitboxes()
        {
            aiHitboxes.UpdateHitboxes();
        }

        // ****************************
        // AIHealth Functions
        // ****************************

        public bool IsDead()
        {
            return aiHealth.IsDead();
        }

        private void RemoveInactiveReceivedDamageHitboxes()
        {
            aiHealth.RemoveInactiveReceivedDamageHitboxes();
        }

        // ****************************
        // UTIL FUNCTIONS
        // ****************************

        public virtual bool IsAgentWithinCameraBounds()
        {
            return aiUtil.IsAgentWithinCameraBounds();
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
