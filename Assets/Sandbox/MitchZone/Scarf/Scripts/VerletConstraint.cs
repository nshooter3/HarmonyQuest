using UnityEngine;
using GameManager;

public class VerletConstraint : ManageableObject
{
    public Transform t2;
    public float distance, compensate1, compensate2, gravityCompensation;
    public bool usesGravity;

    public Vector3 grav;
    private Transform t1;
    private bool isPassed = false;
    private Vector3 t1Old, t2Old;

    public override void OnStart()
    {
        // grav = new Vector3(0, -gravityCompensation, 0);
        t1 = this.transform;
        t1Old = t1.position;
    }

    // Update is called once per frame
    public override void OnLateUpdate()
    {
        // VerletUpdatePoints();
        FixedDistanceConstraint();
        // passingForce = Vector3.Lerp(passingForce, Vector3.zero, 1f);

    }

    public Vector3 Damp(Vector3 source, Vector3 target, float smoothing, float dt)
    {
        return Vector3.Lerp(source, target, 1 - Mathf.Pow(smoothing, dt));
    }

    private void VerletUpdatePoints()
    {
        t1.position = Damp(t1.position, t1.position - t1Old, 0.5f, Time.deltaTime);
        t1Old = t1.position;
    }

    public void FixedDistanceConstraint()
    {
        Vector3 delta = t1.position - t2.position;
        float length = delta.magnitude;
        float diff = (length - distance) / length;
        Vector3 gravity = usesGravity ? grav : Vector3.zero;
        Vector3 target = (delta * compensate1 * diff /* - gravity *//* - passingForce*/);
        t1.position = Damp(t1.position, t1.position - target, 0.5f, Time.deltaTime);
    }
}
