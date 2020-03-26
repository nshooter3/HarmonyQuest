namespace Melody.States
{
    public class CounterState : MelodyState
    {
        public CounterState(MelodyController controller) : base(controller) { }

        protected override void Enter()
        {
            melodyController.melodyAnimator.Counter();

        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);
            if (melodyController.melodyAnimator.IsCounterFinishedPlaying())
            {
                nextState = new IdleState(melodyController);
                ableToExit = true;
            }
        }

        public override void OnExit() { }
    }
}
