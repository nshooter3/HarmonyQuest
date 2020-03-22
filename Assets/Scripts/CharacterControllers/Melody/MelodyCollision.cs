namespace Melody
{
    using UnityEngine;

    public class MelodyCollision
    {

        private MelodyController controller;

        private bool isGrounded;
        private bool isSliding;

        //Radian Y value of the steepest slope that can cause the player to slide.
        private float steepestSlopeYValue;
        //Collision normal direction of the steepest slope, used to calculate which direction the player should slide.
        public Vector3 steepestSlopeDirection { get; private set; }

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
            //Only pay attention to slopes steeper than the grounded normal threshold, hence it is our default value.
            steepestSlopeYValue = controller.config.groundedYNormalThreshold;
            steepestSlopeDirection = Vector3.zero;
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;
                //If Melody is colliding with something that has a contact normal y value greater than groundedYNormalThreshold, we consider her grounded.
                isGrounded |= normal.y >= controller.config.groundedYNormalThreshold;
                isSliding |= normal.y >= controller.config.slidingYNormalThreshold;
                //If this slope is steeper than our last one without angling further downwards than sideways, then it is our new steepest slope.
                if (normal.y < steepestSlopeYValue && normal.y > controller.config.slidingYNormalThreshold)
                {
                    steepestSlopeYValue = normal.y;
                    steepestSlopeDirection = collision.GetContact(i).normal;
                }
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

        public bool IsInAir()
        {
            return !isGrounded && !isSliding;
        }
    }
}
