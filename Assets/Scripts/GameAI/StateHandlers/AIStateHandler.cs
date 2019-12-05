namespace GameAI.StateHandlers
{
    using States;

    public class AIStateHandler
    {
        protected AIState currentState;
        protected AIState nextState;

        public void Init(AIStateUpdateData updateData, AIState initState)
        {
            currentState = initState;
            initState.Init(updateData);
        }

        public void OnUpdate(AIStateUpdateData updateData)
        {
            currentState.CheckForStateChange(updateData);
            if (nextState != null && currentState.readyForStateTransition)
            {
                currentState = nextState;
                nextState = null;
                currentState.Init(updateData);
                currentState.OnUpdate(updateData);
            }
            else
            {
                currentState.OnUpdate(updateData);
            }
        }

        public void OnFixedUpdate(AIStateUpdateData updateData)
        {
            currentState.OnFixedUpdate(updateData);
        }

        public void OnBeatUpdate(AIStateUpdateData updateData)
        {
            currentState.OnBeatUpdate(updateData);
        }

        public AIState GetCurrentState()
        {
            return currentState;
        }

        public void RequestStateTransition(AIState nextState, AIStateUpdateData updateData)
        {
            currentState.Abort(updateData);
            this.nextState = nextState;
        }
    }
}
