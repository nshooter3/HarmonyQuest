namespace Melody.States
{
    using UnityEngine;

    public class MovingState : MelodyState
    {
        public MovingState(MelodyController controller) : base(controller) { }

        protected override void Enter() { }

        public Vector3 velocity = Vector2.zero;

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
            else if (melodyController.Move.magnitude == 0 && velocity.magnitude == 0)
            {
                ableToExit = true;
                nextState = new IdleState(melodyController);
            }

            
            //Debug.Log("time: " + time + " melodyController.Move.magnitude: " + melodyController.Move.magnitude);
            
        }

        public override void OnFixedUpdate()
        {
            
            RotatePlayer(melodyController.config.TurningSpeed);
            Vector3 movement = new Vector3(melodyController.Move.x,0, melodyController.Move.z);
            //Debug.Log("movement: " + movement.x);
            movement *= melodyController.config.MaxAcceleration;
            //Debug.Log("acceleration: " + movement.x);
            movement *= Time.fixedDeltaTime;

            velocity = movement.normalized * (movement.magnitude + velocity.magnitude);
            velocity = Vector3.ClampMagnitude(velocity, melodyController.config.MaxSpeed * melodyController.Move.magnitude);
            velocity.Set(velocity.x, melodyController.rigidBody.velocity.y, velocity.z);
            melodyController.rigidBody.velocity = velocity;


            melodyController.animator.SetFloat("Move", velocity.magnitude / melodyController.config.MaxSpeed);

            
            //Debug.Log("Velocity: " + melodyController.rigidBody.velocity);
          
        }

        void RotatePlayer(float turnSpeedModifier)
        {
            //Rotate player to face movement direction
            if (melodyController.Move.magnitude > 0)
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
            melodyController.transform.eulerAngles = new Vector3(0, melodyController.transform.eulerAngles.y, 0);
        }

        public override void OnExit()
        {
            melodyController.animator.SetFloat("Move", 0f);
        }
    }
}