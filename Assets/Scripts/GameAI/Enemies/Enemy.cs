namespace GameAI.Enemies
{
    using UnityEngine;
    public class Enemy : AIAgent
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
        /// How far above the player to position the navPos when tracking them
        /// </summary>
        protected float navPosHeightOffset = 2.25f;

        protected Vector3 moveDirection = Vector3.zero;
        protected Vector3 moveDirectionNoGravity = Vector3.zero;

        protected RigidbodyConstraints defaultConstraints;

        /// <summary>
        /// Whether or not to make navPos visible.
        /// </summary>
        public bool showDestination = false;

        [SerializeField]
        protected Rigidbody rb;

        public override void Init()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }
            navPos.transform.parent = null;
            navPos.SetActive(showDestination);
            defaultConstraints = rb.constraints;

            base.Init();
        }

        protected virtual void Move(Vector3 destination)
        {
            moveDirection = (destination - aiAgentBottom.position).normalized;
            moveDirectionNoGravity = moveDirection;

            // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
            // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
            // as an acceleration (ms^-2)
            moveDirection.y -= gravity * Time.deltaTime;
            rb.velocity = (moveDirection * Time.deltaTime) * speed;
            Rotate(1.0f);
        }

        protected virtual void Rotate(float turnSpeedModifier)
        {
            //Rotate enemy to face movement direction
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
