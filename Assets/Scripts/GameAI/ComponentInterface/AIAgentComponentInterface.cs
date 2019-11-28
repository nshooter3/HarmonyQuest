namespace GameAI.ComponentInterface
{
    using Navigation;
    using GamePhysics;
    using UnityEngine;
    using StateHandlers;

    public abstract class AIAgentComponentInterface : MonoBehaviour
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

        public virtual void Init()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }

            Origin.parent = null;
            if (NavMeshUtil.IsNavMeshBelowTransform(transform, out Vector3 navmeshPosBelowOrigin))
            {
                Origin.transform.position = navmeshPosBelowOrigin;
            }
            else
            {
                Debug.LogError("AgentComponentInterface Init WARNING: Agent origin not located on or above navmesh.");
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

        /// <summary>
        /// Implement this in the child class to specify what kind of state machine this agent will use.
        /// </summary>
        /// <returns> A new instance of this agent's state machine </returns>
        public abstract AIStateHandler GetStateHandler();

        /// <summary>
        /// Implement this in the child class to specify what kind of navigator this agent will use.
        /// </summary>
        /// <returns> A new instance of this agent's navigator </returns>
        public abstract Navigator GetNavigator();
    }
}
