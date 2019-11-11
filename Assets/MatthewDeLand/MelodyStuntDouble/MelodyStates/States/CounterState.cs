public class CounterState : MelodyState
{
    public CounterState(MelodyController controller) : base(controller){}

    protected override void Enter()
    {
        melodyController.animator.SetTrigger("Counter");

    }

    public override void OnUpdate(float time)
    {
        base.OnUpdate(time);
        if (melodyController.animator.IsInTransition(0) && melodyController.animator.GetCurrentAnimatorStateInfo(0).IsName("Counter"))
        {
            nextState = new IdleState(melodyController);
            ableToExit = true;
        }
    }

    public override void OnExit() { }
}
