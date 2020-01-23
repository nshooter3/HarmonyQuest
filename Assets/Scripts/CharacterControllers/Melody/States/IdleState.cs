namespace Melody.States
{
    public class IdleState : MelodyState
    {

        public IdleState(MelodyController controller) : base(controller) { }

        protected override void Enter() { }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);

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
            else if (melodyController.input.GetHorizontalMovement() != 0 || melodyController.input.GetVerticalMovement() != 0)
            {
                ableToExit = true;
                nextState = new MovingState(melodyController);
            }
        }

        public override void OnExit() { }
    }
}
