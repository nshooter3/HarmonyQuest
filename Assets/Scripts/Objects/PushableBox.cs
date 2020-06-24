using UnityEngine;
using GamePhysics;

public class PushableBox : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private Collider col;

    public float pushSpeed;

    [SerializeField]
    private CollisionWrapper collisionWrapper;

    [SerializeField]
    private LayerMask wallCheckMask;

    public bool beingPushed { get; private set; }
    public bool moving { get; private set; }

    private Vector3 pushDisplacement;

    private Vector3 boxcastOrigin;
    private Vector3 boxcastScale;
    private Vector3 boxcastDirection;
    private Quaternion boxcastRotation;
    private float boxcastDistance;

    bool hitObstacleWhileBeingPushed;
    RaycastHit boxcastHit;

    private void Start()
    {
        collisionWrapper.AssignFunctionToCollisionEnterDelegate(Collision);
        collisionWrapper.AssignFunctionToCollisionStayDelegate(Collision);
        collisionWrapper.AssignFunctionToCollisionExitDelegate(CollisionExit);
    }

    private void FixedUpdate()
    {
        beingPushed = false;
    }

    private void Collision(Collision other)
    {
        beingPushed = true;
        pushDisplacement = other.contacts[0].normal * pushSpeed;

        boxcastOrigin = transform.position;
        boxcastScale = transform.localScale;
        boxcastDirection = pushDisplacement.normalized;
        boxcastRotation = Quaternion.FromToRotation(transform.up, pushDisplacement.normalized) * transform.rotation;
        boxcastDistance = pushDisplacement.magnitude;

        //Temporarily disable our collider to prevent our boxcast from hitting the box it's coming from.
        col.enabled = false;

        hitObstacleWhileBeingPushed = Physics.BoxCast(boxcastOrigin, boxcastScale / 2f, boxcastDirection, out boxcastHit, boxcastRotation, boxcastDistance, wallCheckMask);

        col.enabled = true;

        if (!hitObstacleWhileBeingPushed)
        {
            rb.MovePosition(transform.position + pushDisplacement);
            moving = true;
        }
        else
        {
            Debug.DrawRay(boxcastHit.point, Vector3.up * 10f, Color.magenta);
            moving = false;
        }
    }

    private void CollisionExit(Collision other)
    {
        beingPushed = false;
        moving = false;
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
}
