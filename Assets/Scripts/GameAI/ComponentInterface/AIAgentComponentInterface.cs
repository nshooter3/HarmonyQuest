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
        public Transform aggroTarget;

        /// <summary>
        /// A transform that determines where this enemy will return to once disengaged.
        /// If this transform has a parent, it will automatically be unparented once the scene loads.
        /// </summary>
        public Transform origin;

        /// <summary>
        /// Collider that causes the agent to aggro when a target enters it. Goes unused if null.
        /// </summary>
        public CollisionWrapper aggroZone;

        /// <summary>
        /// A Transform stuck to the bottom of our AI agent. This is used to determine agent proximity to target positions.
        /// </summary>
        public Transform aiAgentBottom;

        public bool disengageWithDistance = true;
        public float disengageDistance = 15.0f;

        public bool targetInLineOfSight = false;

        /// <summary>
        /// How fast this enemy moves.
        /// </summary>
        public float speed;

        /// <summary>
        /// Gravity's effect on this enemy.
        /// </summary>
        public Vector3 gravity = new Vector3(0, -20, 0);

        /// <summary>
        /// How fast this enemy rotates
        /// </summary>
        public float rotateSpeed;

        /// <summary>
        /// Debug sphere gameobject to show where the enemy is attempting to navigate.
        /// </summary>
        public GameObject navPos;
        /// <summary>
        /// How far above the player to position the navPos when tracking them
        /// </summary>
        public float navPosHeightOffset = 2.25f;

        protected Vector3 moveDirection = Vector3.zero;
        protected Vector3 rotationDirection = Vector3.zero;
        private Vector3 newVelocity = Vector3.zero;
        private float prevYVel = 0;

        public RigidbodyConstraints defaultConstraints { get; private set; }

        /// <summary>
        /// Whether or not to make navPos visible.
        /// </summary>
        public bool showDestination = false;

        [SerializeField]
        protected Rigidbody rb;

        public virtual void Init()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }

            origin.parent = null;
            if (NavMeshUtil.IsNavMeshBelowTransform(transform, out Vector3 navmeshPosBelowOrigin))
            {
                origin.transform.position = navmeshPosBelowOrigin;
            }
            else
            {
                Debug.LogError("AgentComponentInterface Init WARNING: Agent origin not located on or above navmesh.");
            }

            navPos.transform.parent = null;
            navPos.SetActive(showDestination);
            defaultConstraints = rb.constraints;

            aggroTarget = TestPlayer.instance.transform;
        }

        public virtual void Move(Vector3 destination, bool ignoreYValue = true)
        {
            moveDirection = (destination - aiAgentBottom.position).normalized;

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
            rb.velocity = newVelocity;
        }

        public virtual void ApplyGravity()
        {
            // Apply a force directly so we can handle gravity on our own instead of relying on rigidbody gravity.
            rb.AddForce(gravity, ForceMode.Acceleration);
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
            SetRigidbodyConstraints(defaultConstraints);
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
