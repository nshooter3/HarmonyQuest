namespace Melody.States
{
    using UnityEngine;

    public class DodgeState : MelodyState
    {

        protected Vector3 Dodge = Vector3.left;

        public DodgeState(MelodyController controller) : base(controller)
        {
        }

        protected override void Enter()
        {
            melodyController.animator.SetTrigger("Dodge");
            melodyController.rigidBody.useGravity = false;
            Dodge = new Vector3(1 * Mathf.Sin(Mathf.Deg2Rad * melodyController.transform.eulerAngles.y), 0, Mathf.Cos(Mathf.Deg2Rad * melodyController.transform.eulerAngles.y));
            Dodge *= 4;
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);

            melodyController.rigidBody.velocity = Dodge;
            if (melodyController.animator.IsInTransition(0) && melodyController.animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
            {
                nextState = new IdleState(melodyController);
                ableToExit = true;
            }
        }

        public override void OnExit()
        {
            melodyController.rigidBody.useGravity = true;
            melodyController.rigidBody.velocity = Vector3.zero;
            melodyController.animator.ResetTrigger("Dodge");
        }
    }
}
