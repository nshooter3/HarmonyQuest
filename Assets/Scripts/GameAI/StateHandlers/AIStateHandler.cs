namespace GameAI.StateHandlers
{
    using AIStates;

    public class AIStateHandler
    {
        protected AIState currentState;
        protected AIState nextState;
        protected AIState prevState;

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
                prevState = currentState;
                currentState = nextState;
                nextState = null;
                currentState.Init(updateData);
            }
            currentState.OnUpdate(updateData);
            updateData.aiGameObjectFacade.UpdateSubclasses();
        }

        public void OnFixedUpdate(AIStateUpdateData updateData)
        {
            currentState.OnFixedUpdate(updateData);
            updateData.aiGameObjectFacade.FixedUpdateSubclasses();
        }

        public void OnBeatUpdate(AIStateUpdateData updateData)
        {
            currentState.OnBeatUpdate(updateData);
        }

        public AIState GetCurrentState()
        {
            return currentState;
        }

        public AIState GetPrevState()
        {
            return prevState;
        }

        public void RequestStateTransition(AIState nextState, AIStateUpdateData updateData)
        {
            currentState.Abort(updateData);
            this.nextState = nextState;
        }
    }
}
