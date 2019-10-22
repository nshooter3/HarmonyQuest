namespace AI
{
    using UnityEngine;

    public class BasicNavMeshEnemy : AggroableEnemy
    {
        public Transform bottom;

        public GameObject navPos;

        public float speed;
        public float gravity;
        public float rotateSpeed;

        public bool showDestination = false;

        [SerializeField]
        private Rigidbody rb;

        private Vector3 moveDirection = Vector3.zero;
        private Vector3 moveDirectionNoGravity = Vector3.zero;

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            SetTarget(bottom, aggroTarget);
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }
            navPos.transform.parent = null;
            navPos.SetActive(showDestination);
        }

        // Update is called once per frame
        new void Update()
        {
            base.Update();
            Move();
        }

        void Move()
        {
            if (aggroState == AggroState.navigateToTarget)
            {
                Vector3 destination = GetNextWaypoint();
                navPos.transform.position = destination;
                //print("DESTINATION = " + destination);
                //print("BOTTOM POS = " + bottom.position);
                moveDirection = (destination - bottom.position).normalized;

                moveDirectionNoGravity = moveDirection;

                // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
                // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
                // as an acceleration (ms^-2)
                moveDirection.y -= gravity * Time.deltaTime;

                // Move the controller
                rb.velocity = (moveDirection * Time.deltaTime) * speed;
                //print("RB VELOCITY: " + rb.velocity);

                RotateEnemy(1.0f);
            }
            else if (aggroState == AggroState.engageTarget)
            {
                navPos.transform.position = new Vector3(aggroTarget.transform.position.x, aggroTarget.transform.position.y + 2.25f, aggroTarget.transform.position.z);
                moveDirection = (aggroTarget.transform.position - transform.position).normalized;

                moveDirectionNoGravity = moveDirection;

                // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
                // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
                // as an acceleration (ms^-2)
                moveDirection.y -= gravity * Time.deltaTime;

                // Move the controller
                rb.velocity = (moveDirection * Time.deltaTime) * speed;

                RotateEnemy(1.0f);
            }
            else if (aggroState == AggroState.idle || aggroState == AggroState.deAggro)
            {
                rb.velocity = Vector3.zero;
            }

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
