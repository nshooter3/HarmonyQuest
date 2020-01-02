namespace GameAI.AIGameObjects
{
    using Navigation;
    using GamePhysics;
    using UnityEngine;
    using GameAI.States;

    public abstract class AIGameObject : MonoBehaviour
    {
        /// <summary>
        /// The transform this agent is attempting to reach when aggroed.
        /// </summary>
        public Transform AggroTarget { get; private set; }

        /// <summary>
        /// A transform that determines where this enemy will return to once disengaged.
        /// If this transform has a parent, it will automatically be unparented once the scene loads.
        /// </summary>
        [SerializeField]
        [Tooltip("A transform that determines where this enemy will return to once disengaged. If this transform has a parent, it will automatically be unparented once the scene loads.")]
        private Transform origin;
        public Transform Origin { get => origin; private set => origin = value; }

        /// <summary>
        /// Collider that causes the agent to aggro when a target enters it. Goes unused if null.
        /// </summary>
        [SerializeField]
        [Tooltip("Collider that causes the agent to aggro when a target enters it. Goes unused if null.")]
        private CollisionWrapper aggroZone;
        public CollisionWrapper AggroZone { get => aggroZone; private set => aggroZone = value; }

        /// <summary>
        /// A Transform stuck to the bottom of our AI agent. This is used to determine agent proximity to target positions.
        /// </summary>
        [SerializeField]
        [Tooltip("A Transform stuck to the bottom of our AI agent. This is used to determine agent proximity to target positions.")]
        private Transform aiAgentBottom;
        public Transform AIAgentBottom { get => aiAgentBottom; private set => aiAgentBottom = value; }

        /// <summary>
        /// The rigidbody for our agent
        /// </summary>
        [SerializeField]
        [Tooltip("The rigidbody for our agent")]
        protected Rigidbody rb;

        /// <summary>
        /// The collider for our agent
        /// </summary>
        [SerializeField]
        [Tooltip("The collider for our agent")]
        protected Collider agentCollider;

        /// <summary>
        /// Whether or not the agent should deaggro once the player gets a certain distance away.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether or not the agent should deaggro once the player gets a certain distance away.")]
        private bool disengageWithDistance = true;
        public bool DisengageWithDistance { get => disengageWithDistance; private set => disengageWithDistance = value; }

        /// <summary>
        /// The distance at which the enemy will deaggro if disengageWithDistance is true.
        /// </summary>
        [SerializeField]
        [Tooltip("The distance at which the enemy will deaggro if disengageWithDistance is true.")]
        private float disengageDistance = 15.0f;
        public float DisengageDistance { get => disengageDistance; private set => disengageDistance = value; }

        /// <summary>
        /// How fast this enemy moves.
        /// </summary>
        [SerializeField]
        [Tooltip("How fast this enemy moves.")]
        private float speed;

        /// <summary>
        /// How much this enemy's velocity can change on the x or z axis per physics update.
        /// </summary>
        [SerializeField]
        [Tooltip("How much this enemy's velocity can change on the x or z axis per physics update.")]
        private float maxVelocityChange;

        /// <summary>
        /// Gravity's effect on this enemy.
        /// </summary>
        [SerializeField]
        [Tooltip("Gravity's effect on this enemy.")]
        private Vector3 gravity = new Vector3(0, -20, 0);

        /// <summary>
        /// How fast this enemy rotates
        /// </summary>
        [SerializeField]
        [Tooltip("How fast this enemy rotates")]
        private float rotateSpeed;

        /// <summary>
        /// Whether or not to make navPos visible. This shows where the enemy is attempting to navigate.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether or not to make navPos visible. This shows where the enemy is attempting to navigate.")]
        private bool showDestination = false;

        /// <summary>
        /// Debug sphere gameobject to show where the enemy is attempting to navigate. Visible if showDestination is set to true.
        /// </summary>
        [SerializeField]
        [Tooltip("Debug sphere gameobject to show where the enemy is attempting to navigate. Visible if showDestination is set to true.")]
        private GameObject navPos;
        public GameObject NavPos { get => navPos; private set => navPos = value; }

        /// <summary>
        /// How far above the player to position the navPos when tracking them.
        /// </summary>
        [SerializeField]
        [Tooltip("How far above the player to position the navPos when tracking them.")]
        private float navPosHeightOffset = 2.25f;
        public float NavPosHeightOffset { get => navPosHeightOffset; private set => navPosHeightOffset = value; }

        public RigidbodyConstraints DefaultConstraints { get; private set; }

        [HideInInspector]
        public bool targetInLineOfSight = false;

        protected Vector3 moveDirection = Vector3.zero;
        protected Vector3 rotationDirection = Vector3.zero;
        private Vector3 newVelocity = Vector3.zero;
        private Vector3 velocityChange = Vector3.zero;
        private float prevYVel = 0;

        //Additional movement forces that make an agent attempt to keep away from other agents and obstacles.
        protected Vector3 collisionAvoidanceForce = Vector3.zero;
        protected Vector3 obstacleAvoidanceForce = Vector3.zero;

        //Adjusted avoidance forces that are decreased as the angle between them and the movement direction decreases.
        protected Vector3 adjustedCollisionAvoidanceForce = Vector3.zero;
        protected Vector3 adjustedObstacleAvoidanceForce = Vector3.zero;

        //Private variables used in the AdjustAvoidanceForceBasedOnMovementVelocity to store intermediate results.
        private Vector3 adjustedForce;
        private Vector3 difference;

        public bool debugFlocking = false;

        public virtual void Init()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }

            if (agentCollider == null)
            {
                agentCollider = GetComponent<Collider>();
            }

            Origin.parent = null;
            if (NavMeshUtil.IsNavMeshBelowTransform(transform, out Vector3 navmeshPosBelowOrigin))
            {
                Origin.transform.position = navmeshPosBelowOrigin;
            }
            else
            {
                Debug.LogError("AIGameObject Init WARNING: Agent origin not located on or above navmesh.");
            }

            NavPos.transform.parent = null;
            NavPos.SetActive(showDestination);
            DefaultConstraints = rb.constraints;

            AggroTarget = TestPlayer.instance.transform;
        }

        public virtual void SetVelocity(Vector3 destination, bool ignoreYValue = true)
        {
            moveDirection = (destination - AIAgentBottom.position).normalized;

            rotationDirection = moveDirection;
            rotationDirection.y = 0;
            rotationDirection.Normalize();
            Rotate(rotationDirection, 1.0f);

            if (ignoreYValue)
            {
                prevYVel = rb.velocity.y;
                newVelocity = (moveDirection * Time.deltaTime) * speed;
                newVelocity.y = prevYVel;
            }
            else
            {
                newVelocity = (moveDirection * Time.deltaTime) * speed;
            }

            adjustedCollisionAvoidanceForce = AdjustAvoidanceForceBasedOnMovementVelocity(collisionAvoidanceForce, newVelocity);
            adjustedObstacleAvoidanceForce = AdjustAvoidanceForceBasedOnMovementVelocity(obstacleAvoidanceForce, newVelocity);

            if (debugFlocking)
            {
                Debug.Log("VELOCITY FORCE: " + newVelocity.magnitude);
                Debug.Log("ADJUSTED COLLISION AVOIDANCE FORCE: " + adjustedCollisionAvoidanceForce.magnitude);
                Debug.Log("ADJUSTED OBSTACLE AVOIDANCE FORCE: " + adjustedObstacleAvoidanceForce.magnitude);
            }

            newVelocity = newVelocity + adjustedCollisionAvoidanceForce + adjustedObstacleAvoidanceForce;
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

        public virtual void ApplyVelocity(bool ignoreYValue = true)
        {
            velocityChange = newVelocity - rb.velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            if (ignoreYValue)
            {
                velocityChange.y = 0;
            }
            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        public virtual void ApplyGravity()
        {
            // Apply a force directly so we can handle gravity on our own instead of relying on rigidbody gravity.
            rb.AddForce(gravity, ForceMode.Acceleration);
        }

        public virtual void ResetVelocity()
        {
            newVelocity = Vector3.zero;
        }

        public void SetCollisionAvoidanceForce(Vector3 collisionAvoidanceForce)
        {
            this.collisionAvoidanceForce = collisionAvoidanceForce;
        }

        public void SetObstacleAvoidanceForce(Vector3 obstacleAvoidanceForce)
        {
            this.obstacleAvoidanceForce = obstacleAvoidanceForce;
        }

        public virtual void Rotate(Vector3 direction, float turnSpeedModifier)
        {
            //Rotate enemy to face movement direction
            if (direction.magnitude > 0)
            {
                Vector3 targetPos = transform.position + direction;
                Vector3 targetDir = targetPos - transform.position;

                // The step size is equal to speed times frame time.
                float step = rotateSpeed * turnSpeedModifier * Time.deltaTime;

                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
                Debug.DrawRay(transform.position, newDir, Color.red);

                // Move our position a step closer to the target.
                transform.rotation = Quaternion.LookRotation(newDir);
            }
            //Failsafe to ensure that x and z are always zero.
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

        public void SetRigidbodyConstraints(RigidbodyConstraints constraints)
        {
            rb.constraints = constraints;
        }

        public void SetRigidBodyConstraintsToDefault()
        {
            SetRigidbodyConstraints(DefaultConstraints);
        }

        public void SetRigidBodyConstraintsToLockAllButGravity()
        {
            SetRigidbodyConstraints(RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ);
        }

        public Collider GetCollider()
        {
            return agentCollider;
        }

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
    }
}
