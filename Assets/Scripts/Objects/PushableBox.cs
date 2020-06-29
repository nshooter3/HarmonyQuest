namespace Objects
{
    using UnityEngine;
    using Melody;
    using HarmonyQuest;

    public class PushableBox : PhysicsObject
    {
        [SerializeField]
        private Rigidbody rb;

        [SerializeField]
        private Collider col;

        public float pushSpeed;

        [SerializeField]
        private LayerMask wallCheckMask;

        [HideInInspector]
        public bool moveThisFrame;

        public bool beingPushed { get; private set; }
        public bool moving { get; private set; }
        private bool falling;
        private bool grounded;

        private Vector3 pushDisplacement;

        private Vector3 boxcastOrigin;
        private Vector3 boxcastExtents;
        private Vector3 boxcastDirection;
        private Quaternion boxcastRotation;
        private float boxcastDistance;

        private bool hitDetected;
        private RaycastHit boxcastHit;
        private RaycastHit[] boxcastHits;

        private bool gravityBoxcastHitGround;
        private float gravitySpeed = 0f;
        private float minGravitySpeed = 0.15f;
        private float gravityMaxSpeed = 0.5f;
        private float gravityAcceleration = 0.01f;
        private float gravityPosOffset = 0.005f;

        private float boxCastAboveRaycastDistance = 0.1f;

        private Vector3 newPosition;

        private MelodyController melodyController;

        [SerializeField]
        private bool debug;

        //Determine which direction the box should be pushed based on which trigger the player is in.
        [SerializeField]
        private PushableBoxTrigger[] pushableBoxTriggers;

        private PushableBox aboveBox;
        bool hasBeenMovedFromBelow = false;

        private void Start()
        {
            gravitySpeed = minGravitySpeed;
            melodyController = ServiceLocator.instance.GetMelodyController();
        }

        public override void ObjectFixedUpdate()
        {
            PhysicsTick();
        }

        public override void ObjectLateFixedUpdate()
        {
            ApplyMove();
        }

        public void SetParams()
        {
            beingPushed = false;
            moving = false;

            boxcastOrigin = transform.position;
            boxcastExtents = (transform.localScale / 2f) * 0.99f;
            boxcastRotation = transform.rotation;

            newPosition = transform.position;
        }

        public void PhysicsTick()
        {
            if (hasBeenMovedFromBelow == false)
            {
                SetParams();

                if (moveThisFrame == true)
                {
                    CalculateMoveFromTriggers();
                }
                CalculateGravity();
            }
        }

        public void ApplyMove()
        {
            if (transform.position != newPosition)
            {
                rb.MovePosition(newPosition);
            }
            hasBeenMovedFromBelow = false;
        }

        public void CalculateMoveFromTriggers()
        {
            beingPushed = true;

            pushDisplacement = GetPushDirectionFromTriggers().normalized * pushSpeed;
            CaculateMove(pushDisplacement);
        }

        private void CaculateMove(Vector3 movement)
        {
            boxcastDirection = movement.normalized;
            boxcastDistance = movement.magnitude;

            //Temporarily disable our collider to prevent our boxcast from hitting the box it's coming from.
            col.enabled = false;

            hitDetected = Physics.BoxCast(boxcastOrigin, boxcastExtents, boxcastDirection, out boxcastHit, boxcastRotation, boxcastDistance, wallCheckMask);

            col.enabled = true;

            if (!hitDetected)
            {
                newPosition = boxcastOrigin + movement;
                moving = true;
                ApplyMovementToBoxesSittingOnTopOfThisOne(movement);
            }
            else
            {
                Debug.DrawRay(boxcastHit.point, Vector3.up * 10f, Color.magenta);
                moving = false;
            }
            moveThisFrame = false;
        }

        private void CalculateGravity()
        {
            falling = false;

            if (debug)
            {
                Debug.Log("GRAVITY SPEED: " + gravitySpeed);
            }

            //Temporarily disable our collider to prevent our boxcast from hitting the box it's coming from.
            col.enabled = false;
            hitDetected = Physics.BoxCast(boxcastOrigin, boxcastExtents, Vector3.down, out boxcastHit, boxcastRotation, gravitySpeed);
            col.enabled = true;

            if (hitDetected == true)
            {
                if (grounded == false)
                {
                    gravitySpeed = minGravitySpeed;
                    newPosition.y = boxcastHit.point.y + gravityPosOffset + transform.localScale.y / 2f;
                }
                grounded = true;
            }
            else if (hitDetected == false)
            {
                newPosition.y += gravitySpeed * -1f;
                gravitySpeed = Mathf.MoveTowards(gravitySpeed, gravityMaxSpeed, gravityAcceleration);
                falling = true;
                grounded = false;
            }
        }

        private void ApplyMovementToBoxesSittingOnTopOfThisOne(Vector3 movement)
        {
            //Temporarily disable our collider to prevent our boxcast from hitting the box it's coming from.
            col.enabled = false;
            //Use BoxCastAll to detect multiple boxes on top of this one.
            boxcastHits = Physics.BoxCastAll(boxcastOrigin, boxcastExtents, Vector3.up, boxcastRotation, boxCastAboveRaycastDistance, melodyController.config.boxPushingLayerMask);
            col.enabled = true;

            foreach(RaycastHit aboveHit in boxcastHits)
            {
                aboveBox = aboveHit.transform.GetComponent<PushableBox>();
                if (aboveBox != null)
                {
                    aboveBox.hasBeenMovedFromBelow = true;
                    aboveBox.SetParams();
                    aboveBox.CaculateMove(movement);
                }
            }
        }

        //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;

            if (beingPushed)
            {
                Gizmos.DrawWireCube(boxcastDirection * boxcastDistance, Vector3.one);
            }
            Gizmos.DrawWireCube(Vector3.down * gravitySpeed, Vector3.one);
        }

        private Vector3 GetPushDirectionFromTriggers()
        {
            foreach (PushableBoxTrigger box in pushableBoxTriggers)
            {
                if (box.containsPlayer == true)
                {
                    return box.transform.forward;
                }
            }
            return Vector3.zero;
        }
    }
}
