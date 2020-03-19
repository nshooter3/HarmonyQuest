namespace Melody.States
{
    using System;
    using UnityEngine;

    public class DashOutroState : MelodyState
    {

        protected Vector3 dodge = Vector3.left;
        protected float timer = 0;

        public DashOutroState(MelodyController controller) : base(controller)
        {
        }

        protected override void Enter()
        {
           
            nextState = new IdleState(melodyController);
            timer = 0;
            melodyController.rigidBody.velocity = Vector3.zero;
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);


            timer += time;
            if (timer >= melodyController.config.DashOutroTime)
            {
                nextState = new IdleState(melodyController);
                ableToExit = true;
            }

            //melodyController.animator.SetTrigger("DashOutro");
            melodyController.melodyRenderer.enabled = true;
            melodyController.scarfRenderer.enabled = false;

            melodyController.move *= melodyController.config.MaxSpeed;
            RotatePlayer(3);

            melodyController.move = melodyController.move * time * 100 * 0.5f;
            melodyController.rigidBody.velocity = new Vector3(melodyController.move.x, melodyController.rigidBody.velocity.y, melodyController.move.z);
        }

        void RotatePlayer(float turnSpeedModifier)
        {
            //Rotate player to face movement direction
            if (melodyController.move.magnitude > 0)
            {
                Vector3 targetPos = melodyController.transform.position + melodyController.move;
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
            melodyController.animator.ResetTrigger("Dash");
            melodyController.animator.ResetTrigger("DashOutro");
        }
    }
}
