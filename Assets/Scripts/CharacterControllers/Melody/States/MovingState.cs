namespace Melody.States
{
    using UnityEngine;

    public class MovingState : MelodyState
    {
        public MovingState(MelodyController controller) : base(controller) { }

        protected override void Enter() { }

        public Vector3 velocity = Vector3.zero;
        private Vector3 desiredVelocity = Vector3.zero;
        private Vector3 acceleration = Vector3.zero;
        private float maxSpeedChange;

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);

            //Check For Attack
            if (melodyController.input.AttackButtonDown())
            {
                ableToExit = true;
                nextState = new AttackState(melodyController);
            }
            else if (melodyController.input.ParryButtonDown())
            {
                ableToExit = true;
                nextState = new CounterState(melodyController);
            }
            else if (melodyController.input.DodgeButtonDown())
            {
                ableToExit = true;
                nextState = new DashIntroState(melodyController);
            }
            else if (melodyController.Move.magnitude == 0.0f && velocity.magnitude == 0.0f)
            {
                ableToExit = true;
                nextState = new IdleState(melodyController);
            }

            CalculateVelocity();
        }

        public override void OnFixedUpdate()
        {
            ApplyVelocity();
            ApplyGravity();
        }

        void CalculateVelocity()
        {
            desiredVelocity = new Vector3(melodyController.Move.x, 0, melodyController.Move.z) * melodyController.config.MaxSpeed;
            maxSpeedChange = melodyController.config.MaxAcceleration * Time.deltaTime;
            velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
            velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
            //Keep whatever our rigidbody y velocity was on the last frame to ensure that gravity works properly.
            velocity.y = melodyController.rigidBody.velocity.y;
        }

        void ApplyVelocity()
        {
            RotatePlayer(melodyController.config.TurningSpeed);
            melodyController.rigidBody.velocity = velocity;
            melodyController.animator.SetFloat("Move", desiredVelocity.magnitude / melodyController.config.MaxSpeed);
        }

        void ApplyGravity()
        {
            melodyController.rigidBody.AddForce(melodyController.config.Gravity, ForceMode.Acceleration);
        }

        void RotatePlayer(float turnSpeedModifier)
        {
            //Rotate player to face movement direction
            if (melodyController.Move.magnitude > 0.0f)
            {
                Vector3 targetPos = melodyController.transform.position + melodyController.Move;
                Vector3 targetDir = targetPos - melodyController.transform.position;

                float step = 5 * turnSpeedModifier * Time.deltaTime;

                Vector3 newDir = Vector3.RotateTowards(melodyController.transform.forward, targetDir, step, 0.0f);
                Debug.DrawRay(melodyController.transform.position, newDir, Color.red);

                // Move our position a step closer to the target.
                melodyController.transform.rotation = Quaternion.LookRotation(newDir);
            }
            //Failsafe to ensure that x and z are always zero.
            melodyController.transform.eulerAngles = new Vector3(0.0f, melodyController.transform.eulerAngles.y, 0.0f);
        }

        public override void OnExit()
        {
            melodyController.animator.SetFloat("Move", 0.0f);
        }
    }
}