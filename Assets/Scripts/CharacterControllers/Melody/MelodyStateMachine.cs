namespace Melody.States
{
    public class MelodyStateMachine
    {
        readonly MelodyController Controller;
        MelodyState CurrentState;
        MelodyState NextState;


        public MelodyStateMachine(MelodyController Controller)
        {
            this.Controller = Controller;

            CurrentState = new IdleState(Controller);
        }

        public void OnUpdate(float time)
        {
            CurrentState.OnUpdate(time);
            if(CurrentState.CanExit() && CurrentState.NextState() != null)
            {
                NextState = CurrentState.NextState();
                CurrentState = NextState;
                NextState = null;
            }
        }

        public void OnFixedUpdate()
        {
            CurrentState.OnFixedUpdate();
        }

        public string GetCurrentStateName()
        {
            return CurrentState.stateName;
        }
    }
}