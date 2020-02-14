namespace GameAI.AIGameObjects
{
    using GameAI.Navigation;
    using GamePhysics;
    using UnityEngine;

    [System.Serializable]
    public class AIGameObjectData
    {
        // ****************************
        // Inspector Exposed Parameters
        // ****************************

        /// <summary>
        /// This agent's gameobject.
        /// </summary>
        [Tooltip("This agent's gameobject.")]
        public GameObject gameObject;

        /// <summary>
        /// A transform that determines where this enemy will return to once disengaged.
        /// If this transform has a parent, it will automatically be unparented once the scene loads.
        /// </summary>
        [Tooltip("A transform that determines where this enemy will return to once disengaged. If this transform has a parent, it will automatically be unparented once the scene loads.")]
        public Transform origin;

        /// <summary>
        /// Collider that causes the agent to aggro when a target enters it. Goes unused if null.
        /// </summary>
        [Tooltip("Collider that causes the agent to aggro when a target enters it. Goes unused if null.")]
        public CollisionWrapper aggroZone;

        /// <summary>
        /// A Transform stuck to the bottom of our AI agent. This is used to determine agent proximity to target positions.
        /// </summary>
        [Tooltip("A Transform stuck to the bottom of our AI agent. This is used to determine agent proximity to target positions.")]
        public Transform aiAgentBottom;

        /// <summary>
        /// Set of colliders that the enemy can use to determine which direction they can go when moving freely.
        /// </summary>
        [Tooltip("Set of colliders that the enemy can use to determine which direction they can go when moving freely.")]
        public StrafeHitboxes strafeHitBoxes;

        /// <summary>
        /// The rigidbody for our agent
        /// </summary>
        [Tooltip("The rigidbody for our agent.")]
        public Rigidbody rb;

        /// <summary>
        /// The collider for our agent.
        /// </summary>
        [Tooltip("The collider for our agent.")]
        public Collider agentCollider;

        /// <summary>
        /// Whether or not the agent should deaggro once the player gets a certain distance away.
        /// </summary>
        [Tooltip("Whether or not the agent should deaggro once the player gets a certain distance away.")]
        public bool disengageWithDistance;

        /// <summary>
        /// The distance at which the enemy will deaggro if disengageWithDistance is true.
        /// </summary>
        [Tooltip("The distance at which the enemy will deaggro if disengageWithDistance is true.")]
        public float disengageDistance;

        /// <summary>
        /// How fast this enemy moves.
        /// </summary>
        [Tooltip("How fast this enemy moves.")]
        public float speed;

        /// <summary>
        /// Gravity's effect on this enemy.
        /// </summary>
        [Tooltip("Gravity's effect on this enemy.")]
        public Vector3 gravity = new Vector3(0, -20, 0);

        /// <summary>
        /// How fast this enemy rotates
        /// </summary>
        [Tooltip("How fast this enemy rotates")]
        public float rotateSpeed;

        /// <summary>
        /// Whether or not to make navPos visible. This shows where the enemy is attempting to navigate.
        /// </summary>
        [Tooltip("Whether or not to make navPos visible. This shows where the enemy is attempting to navigate.")]
        public bool showDestination = false;

        public MeshRenderer body;
        public MeshRenderer head;

        /// <summary>
        /// Debug sphere gameobject to show where the enemy is attempting to navigate. Visible if showDestination is set to true.
        /// </summary>
        [Tooltip("Debug sphere gameobject to show where the enemy is attempting to navigate. Visible if showDestination is set to true.")]
        public GameObject navPos;

        /// <summary>
        /// How far above the player to position the navPos when tracking them.
        /// </summary>
        [Tooltip("How far above the player to position the navPos when tracking them.")]
        public float navPosHeightOffset;

        // ****************************
        // Inspector Exposed Debug Parameters
        // ****************************

        [SerializeField]
        public bool debugFlocking = false;

        [SerializeField]
        public bool debugEngage = false;

        // ****************************
        // Inspector Hidden Parameters
        // ****************************

        /// <summary>
        /// The transform this agent is attempting to reach when aggroed.
        /// </summary>
        [HideInInspector]
        public Transform aggroTarget;

        [HideInInspector]
        public RigidbodyConstraints defaultConstraints;

        [HideInInspector]
        public bool isAggroed = false;

        [HideInInspector]
        public bool permissionToAttack = false;

        [HideInInspector]
        public bool isAttacking = false;

        //Multiplier for the collision avoidance force for this specific enemy
        [HideInInspector]
        public float individualCollisionAvoidanceModifier = 1.0f;

        //Multiplier for the collision avoidance force for this specific enemy
        [HideInInspector]
        public float individualCollisionAvoidanceMaxDistance = NavigatorSettings.collisionAvoidanceDefaultMaxDistance;
    }
}