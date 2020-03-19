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

        public MelodyPhysics(MelodyController controller)
        {
            this.controller = controller;
        }

        public void CalculateVelocity()
        {
            desiredVelocity = new Vector3(controller.move.x, 0, controller.move.z) * controller.config.MaxSpeed;
            maxSpeedChange = controller.config.MaxAcceleration * Time.deltaTime;
            velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
            velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
            //Keep whatever our rigidbody y velocity was on the last frame to ensure that gravity works properly.
            velocity.y = controller.rigidBody.velocity.y;
        }

        public void ApplyVelocity()
        {
            RotatePlayer(controller.config.TurningSpeed);
            controller.rigidBody.velocity = velocity;
            controller.animator.SetFloat("Move", desiredVelocity.magnitude / controller.config.MaxSpeed);
        }

        public void ApplyGravity()
        {
            if (controller.melodyCollision.IsGrounded() == false)
            {
                controller.rigidBody.AddForce(controller.config.Gravity, ForceMode.Acceleration);
            }
        }

        public void RotatePlayer(float turnSpeedModifier)
        {
            //Rotate player to face movement direction
            if (controller.move.magnitude > 0.0f)
            {
                Vector3 targetPos = controller.transform.position + controller.move;
                Vector3 targetDir = targetPos - controller.transform.position;

                float step = controller.config.TurningSpeed * turnSpeedModifier * Time.deltaTime;

                Vector3 newDir = Vector3.RotateTowards(controller.transform.forward, targetDir, step, 0.0f);
                Debug.DrawRay(controller.transform.position, newDir, Color.red);

                // Move our position a step closer to the target.
                controller.transform.rotation = Quaternion.LookRotation(newDir);
            }
            //Failsafe to ensure that x and z are always zero.
            controller.transform.eulerAngles = new Vector3(0.0f, controller.transform.eulerAngles.y, 0.0f);
        }
    }
}
