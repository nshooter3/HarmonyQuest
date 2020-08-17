namespace GameAI.AIGameObjects
{
    using Navigation;
    using UnityEngine;
    using GameAI.AIStates;
    using GamePhysics;
    using HarmonyQuest;
    using HarmonyQuest.Audio;
    using UnityEngine.AI;

    public abstract class AIGameObjectFacade : MonoBehaviour
    {
        [SerializeField]
        public AIGameObjectData data;

        private AIPhysics aiPhysics = new AIPhysics();
        private AIHitboxes aiHitboxes = new AIHitboxes();
        private AIHealth aiHealth = new AIHealth();
        private AIDebug aiDebug = new AIDebug();
        private AIUtil aiUtil = new AIUtil();
        private AISurfaceCollision aiSurfaceCollision = new AISurfaceCollision();
        public AIAnimator aiAnimator;
        public AISound aiSound;
        

        public bool requestingAttackPermission = false;
        public bool attackPermissionGranted = false;
        public bool attacking = false;
        public bool isAvailableToAttack = false;
        public bool shouldAttackAsSoonAsPossible = false;

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

        /// <summary>
        /// Implement this in the child class to specify what kind of animator this agent will use.
        /// </summary>
        /// <returns> A new instance of this agent's animator </returns>
        public abstract AIAnimator GetAnimator();

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

            if (data.strafeHitboxes != null)
            {
                data.strafeHitboxes.Init();
            }

            data.origin.parent = null;
            if (NavMeshUtil.IsAgentOnNavMesh(transform.position, out NavMeshHit hit))
            {
                data.origin.transform.position = hit.position;
            }
            else
            {
                Debug.LogWarning("AIGameObject Init WARNING: Agent origin not located on or above navmesh.");
            }

            data.navPos.transform.parent = null;
            data.navPos.SetActive(data.showDestination);
            data.defaultConstraints = data.rb.constraints;

            data.aggroTarget = ServiceLocator.instance.GetMelodyInfo().GetTransform();

            aiPhysics.Init(this, data);
            aiHitboxes.Init(data);
            aiHealth.Init(data);
            aiDebug.Init(data);
            aiUtil.Init(data);
            aiSurfaceCollision.Init(this, data);

            if (aiSound == null)
            {
                aiSound = GetComponentInChildren<AISound>();
                if (aiSound == null)
                {
                    Debug.LogError("AIGameObject Init WARNING: Agent does not have an aiSound component.");
                }
            }
            aiSound.Init(data);

            AssignFunctionToDamageHitboxFmodCallback(aiSound.PlayFmodEvent);

            DamageReceiver damageReceiver;

            foreach (Collider hurtbox in data.hurtboxes)
            {
                //Create and attach a DamageReceiver to all our hurtboxes at runtime
                damageReceiver = hurtbox.gameObject.AddComponent<DamageReceiver>();
                damageReceiver.AssignFunctionToReceiveDamageDelegate(aiHealth.ReceiveDamageHitbox);
            }

            if (aiAnimator == null)
            {
                aiAnimator = new AIAnimator(GetComponent<Animator>());
            }
        }

        public void UpdateSubclasses()
        {
            if (IsDead() == false)
            {
                UpdateHitboxes();
                RemoveInactiveReceivedDamageHitboxes();
                UpdateAnimations();
            }
        }

        public void FixedUpdateSubclasses()
        {
            if (IsDead() == false)
            {
                FixedUpdateHealth();
                FixedUpdateSurfaceCollision();
            }
        }

        // ****************************
        // PHYSICS FUNCTIONS
        // ****************************

        public void ResetDesiredVelocity()
        {
            aiPhysics.ResetDesiredVelocity();
        }

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
            aiPhysics.CalculateVelocity(velocity, ignoreYValue, speedModifier, alwaysFaceTarget);
        }

        public virtual void ApplyVelocity(bool ignoreYValue = true, bool applyRotation = true, float turnSpeedModifier = 1.0f, bool instantlyFaceDirection = false)
        {
            aiAnimator.SetVelocity(transform.forward, aiPhysics.GetVelocity(), data.aiStats.speed);
            aiPhysics.ApplyVelocity(ignoreYValue, applyRotation, turnSpeedModifier, instantlyFaceDirection);
        }

        public virtual void ApplyAnimationVelocity()
        {
            aiAnimator.SetVelocity(transform.forward, aiPhysics.GetVelocity(), data.aiStats.speed);
        }

        public virtual void ApplyGravity(Vector3 gravity, bool isIdle = false)
        {
            aiPhysics.ApplyGravity(gravity, isIdle);
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

        public virtual void Rotate(float turningSpeed, bool stationaryTurn = false, Vector3? directionOverride = null)
        {
            aiPhysics.Rotate(turningSpeed, stationaryTurn, directionOverride);
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
            return aiPhysics.GetVelocity().normalized;
        }

        public virtual Vector3 GetTransformForward()
        {
            return aiPhysics.GetTransformForward();
        }

        public PhysicsEntity GetPhysicsEntity()
        {
            return aiPhysics.GetPhysicsEntity();
        }

        // ****************************
        // HITBOX FUNCTIONS
        // ****************************

        public virtual Collider GetCollisionAvoidanceHitbox()
        {
            return aiHitboxes.GetCollisionAvoidanceHitbox();
        }

        public virtual void ActivateHitbox(string name, float delay, float lifetime, int damage, bool counterable = true)
        {
            aiHitboxes.ActivateHitbox(name, delay, lifetime, damage, counterable);
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

        public void DamageHitboxFmodCallback(string fmodEventName)
        {
            aiHitboxes.damageHitboxFmodCallback(fmodEventName);
        }

        public void AssignFunctionToDamageHitboxFmodCallback(AIHitboxes.DamageHitboxFmodCallback func)
        {
            aiHitboxes.AssignFunctionToDamageHitboxFmodCallback(func);
        }

        public void RemoveFunctionFromDamageHitboxFmodCallback(AIHitboxes.DamageHitboxFmodCallback func)
        {
            aiHitboxes.RemoveFunctionFromDamageHitboxFmodCallback(func);
        }

        public void ClearDamageHitboxFmodCallback()
        {
            aiHitboxes.ClearDamageHitboxFmodCallback();
        }

        // ****************************
        // HEALTH FUNCTIONS
        // ****************************

        public bool TookDamageFromPlayerThisFrame()
        {
            return aiHealth.TookDamageFromPlayerThisFrame();
        }

        public bool IsDead()
        {
            return aiHealth.IsDead();
        }

        private void RemoveInactiveReceivedDamageHitboxes()
        {
            aiHealth.RemoveInactiveReceivedDamageHitboxes();
        }

        private void FixedUpdateHealth()
        {
            aiHealth.OnFixedUpdate();
        }

        // ****************************
        // UTIL FUNCTIONS
        // ****************************

        public virtual bool IsAgentWithinCameraBounds()
        {
            return aiUtil.IsAgentWithinCameraBounds();
        }

        // ****************************
        // SOUND FUNCTIONS
        // ****************************

        public void PlayFmodEvent(string eventName)
        {
            aiSound.PlayFmodEvent(eventName);
        }

        public void PlayFmodEvent(string eventName, FmodParamData[] extraParams)
        {
            aiSound.PlayFmodEvent(eventName, extraParams);
        }

        // ****************************
        // ANIMATION FUNCTIONS
        // ****************************
        public void UpdateAnimations()
        {
            aiAnimator.OnUpdate();
        }

        // ****************************
        // SURFACE COLLISION FUNCTIONS
        // ****************************
        public void FixedUpdateSurfaceCollision()
        {
            aiSurfaceCollision.OnFixedUpdate();
        }

        public bool IsGrounded()
        {
            return aiSurfaceCollision.IsGrounded();
        }

        public bool IsSliding()
        {
            return aiSurfaceCollision.IsSliding();
        }

        public bool IsInAir()
        {
            return aiSurfaceCollision.IsInAir();
        }

        public float GetSlopeNormalDotProduct()
        {
            return aiSurfaceCollision.GetSlopeNormalDotProduct();
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
