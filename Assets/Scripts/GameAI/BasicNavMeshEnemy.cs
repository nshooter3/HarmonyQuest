namespace GameAI
{
    using UnityEngine;

    public class BasicNavMeshEnemy : AggroableEnemy
    {

        /// <summary>
        /// How fast this enemy moves.
        /// </summary>
        public float speed;

        /// <summary>
        /// Gravity's effect on this enemy.
        /// </summary>
        public float gravity;

        /// <summary>
        /// How fast this enemy rotates
        /// </summary>
        public float rotateSpeed;

        /// <summary>
        /// Debug sphere gameobject to show where the enemy is attempting to navigate.
        /// </summary>
        public GameObject navPos;

        /// <summary>
        /// Whether or not to make navPos visible.
        /// </summary>
        public bool showDestination = false;

        [SerializeField]
        private Rigidbody rb;

        private Vector3 moveDirection = Vector3.zero;
        private Vector3 moveDirectionNoGravity = Vector3.zero;

        private RigidbodyConstraints defaultConstraints;

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }
            navPos.transform.parent = null;
            navPos.SetActive(showDestination);
            defaultConstraints = rb.constraints;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        // Update is called once per frame
        new void Update()
        {
            base.Update();
            Move();
        }

        void Move()
        {
            if (aggroState == AggroState.idle)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
                return;
            }
            else
            {
                rb.constraints = defaultConstraints;
            }

            Vector3 destination = transform.position;

            if (aggroState == AggroState.engageTarget)
            {
                destination = aggroTarget.transform.position;
                navPos.transform.position = new Vector3(aggroTarget.transform.position.x, aggroTarget.transform.position.y + 2.25f, aggroTarget.transform.position.z);
            }
            else if (aggroState == AggroState.navigateToTarget || aggroState ==  AggroState.deAggro)
            {
                destination = GetNextWaypoint();
                navPos.transform.position = destination;
            }

            moveDirection = (destination - navigationAgentBottom.position).normalized;
            moveDirectionNoGravity = moveDirection;

            // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
            // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
            // as an acceleration (ms^-2)
            moveDirection.y -= gravity * Time.deltaTime;
            rb.velocity = (moveDirection * Time.deltaTime) * speed;
            RotateEnemy(1.0f);
        }

        void RotateEnemy(float turnSpeedModifier)
        {
            //Rotate player to face movement direction
            if (moveDirectionNoGravity.magnitude > 0)
            {
                Vector3 targetPos = transform.position + moveDirectionNoGravity;
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
    }
}
