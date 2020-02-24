using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneMomentum : MonoBehaviour
{

    private Transform thisParent;
    private Rigidbody thisRigidbody;

    private Vector3 parentPosLastFrame = Vector3.zero;

    void Awake()
    {
        thisParent = transform.parent;
        thisRigidbody = transform.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        thisRigidbody.AddForce ( ( parentPosLastFrame - thisParent.position ) * 100 );
        parentPosLastFrame = thisParent.position;
    }
}
