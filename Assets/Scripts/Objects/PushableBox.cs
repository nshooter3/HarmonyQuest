namespace Objects
{
    using UnityEngine;
    using Melody;

    public class PushableBox : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody rb;

        [SerializeField]
        private Collider col;

        public float pushSpeed;

        [SerializeField]
        private LayerMask wallCheckMask;

        public bool beingPushed { get; private set; }
        public bool moving { get; private set; }

        private Vector3 pushDisplacement;

        private Vector3 boxcastOrigin;
        private Vector3 boxcastExtents;
        private Vector3 boxcastDirection;
        private Quaternion boxcastRotation;
        private float boxcastDistance;

        bool hitObstacleWhileBeingPushed;
        RaycastHit boxcastHit;

        private MelodyController melodyController;

        //Determine which direction the box should be pushed based on which trigger the player is in.
        [SerializeField]
        private PushableBoxTrigger[] pushableBoxTriggers;

        private void FixedUpdate()
        {
            beingPushed = false;
            moving = false;
        }

        public void Move()
        {
            beingPushed = true;

            pushDisplacement = GetPushDirectionFromTriggers().normalized * pushSpeed;
            boxcastOrigin = transform.position;
            boxcastExtents = (transform.localScale / 2f) * 0.99f;
            boxcastDirection = pushDisplacement.normalized;
            boxcastRotation = transform.rotation;
            boxcastDistance = pushDisplacement.magnitude;

            //Temporarily disable our collider to prevent our boxcast from hitting the box it's coming from.
            col.enabled = false;

            hitObstacleWhileBeingPushed = Physics.BoxCast(boxcastOrigin, boxcastExtents, boxcastDirection, out boxcastHit, boxcastRotation, boxcastDistance, wallCheckMask);

            col.enabled = true;

            if (!hitObstacleWhileBeingPushed)
            {
                rb.MovePosition(boxcastOrigin + pushDisplacement);
                moving = true;
            }
            else
            {
                Debug.DrawRay(boxcastHit.point, Vector3.up * 10f, Color.magenta);
                moving = false;
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
