namespace Melody
{
    using UnityEngine;

    public class MelodyPhysics
    {
        private MelodyController controller;

        public Vector3 velocity = Vector3.zero;
        private Vector3 desiredVelocity = Vector3.zero;
        private Vector3 acceleration = Vector3.zero;
        private float maxSpeedChange;
        private RaycastHit hit;

        public MelodyPhysics(MelodyController controller)
        {
            this.controller = controller;
        }

        public void CalculateVelocity(float maxSpeed, float maxAcceleration)
        {
            desiredVelocity = new Vector3(controller.move.x, 0, controller.move.z) * maxSpeed;
            maxSpeedChange = maxAcceleration * Time.deltaTime;
            velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
            velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
            //Keep whatever our rigidbody y velocity was on the last frame to ensure that gravity works properly.
            velocity.y = controller.rigidBody.velocity.y;
        }

        public void ApplyVelocity(float maxSpeed, float turningSpeed)
        {
            RotatePlayer(turningSpeed);
            controller.rigidBody.velocity = velocity;
            controller.animator.SetFloat("Move", desiredVelocity.magnitude / maxSpeed);
        }

        public void ApplyGravity(Vector3 gravity)
        {
            if (controller.melodyCollision.IsGrounded() == false)
            {
                controller.rigidBody.AddForce(gravity, ForceMode.Acceleration);
            }
        }

        public void RotatePlayer(float turningSpeed)
        {
            //Rotate player to face movement direction
            if (controller.move.magnitude > 0.0f)
            {
                Vector3 targetPos = controller.transform.position + controller.move;
                Vector3 targetDir = targetPos - controller.transform.position;

                float step = controller.config.TurningSpeed * turningSpeed * Time.deltaTime;

                Vector3 newDir = Vector3.RotateTowards(controller.transform.forward, targetDir, step, 0.0f);
                Debug.DrawRay(controller.transform.position, newDir, Color.red);

                // Move our position a step closer to the target.
                controller.transform.rotation = Quaternion.LookRotation(newDir);
            }
            //Failsafe to ensure that x and z are always zero.
            controller.transform.eulerAngles = new Vector3(0.0f, controller.transform.eulerAngles.y, 0.0f);
        }

        public void SnapToGround()
        {
            if (Physics.Raycast(controller.transform.position, Vector3.down, out hit, controller.config.snapToGroundRaycastDistance, controller.config.snapToGroundLayerMask))
            {
                controller.transform.position = hit.point + controller.config.snapOffset;
            }
        }
    }
}
