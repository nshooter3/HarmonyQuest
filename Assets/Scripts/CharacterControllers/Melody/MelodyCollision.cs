namespace Melody
{
    using UnityEngine;

    public class MelodyCollision
    {

        private MelodyController controller;

        private bool isGrounded;
        private bool isSliding;

        public MelodyCollision(MelodyController controller)
        {
            this.controller = controller;
            this.controller.melodyColliderWrapper.AssignFunctionToCollisionEnterDelegate(SetGrounded);
            this.controller.melodyColliderWrapper.AssignFunctionToCollisionStayDelegate(SetGrounded);
        }

        public void OnFixedUpdate()
        {
            //Since FixedUpdate fires before collision functions, we can reset isGrounded here and let our collision enter/stay functions recheck every frame.
            isGrounded = false;
            isSliding = false;
        }

        public void SetGrounded(Collision collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;
                //If Melody is colliding with something that has a contact normal y value greater than groundedYNormalThreshold, we consider her grounded.
                isGrounded |= normal.y >= controller.config.groundedYNormalThreshold;
                isSliding |= normal.y >= controller.config.slidingYNormalThreshold;
            }
        }

        public bool IsGrounded()
        {
            return isGrounded;
        }

        public bool IsSliding()
        {
            return isSliding && !isGrounded;
        }
    }
}
