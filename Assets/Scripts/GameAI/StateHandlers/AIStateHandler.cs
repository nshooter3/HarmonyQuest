namespace GameAI.StateHandlers
{
    using Behaviors;

    public class AIStateHandler
    {
        protected AIBehavior currentState;
        protected AIBehavior nextState;

        public void Init(AIStateUpdateData updateData, AIBehavior initState)
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

        public AIBehavior GetCurrentState()
        {
            return currentState;
        }

        public void RequestStateTransition(AIBehavior nextState, AIStateUpdateData updateData)
        {
            currentState.Abort(updateData);
            this.nextState = nextState;
        }
    }
}
