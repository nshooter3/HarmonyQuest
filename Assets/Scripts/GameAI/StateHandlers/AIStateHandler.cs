namespace GameAI.StateHandlers
{
    using Behaviors;
    using System;

    public abstract class AIStateHandler
    {
        protected AIBehavior currentState;
        protected AIBehavior[] states;

        public abstract void Init();
        protected abstract void CheckForStateChange(AIStateUpdateData updateData);

        public void Update(AIStateUpdateData updateData)
        {
            CheckForStateChange(updateData);
            currentState.Update(updateData);
        }

        public string GetCurrentState()
        {
            return currentState.GetName();
        }

        protected void ChangeState(string newState, AIStateUpdateData updateData)
        {
            currentState.Abort(updateData);
            foreach (AIBehavior state in states)
            {
                if (state.GetName() == newState)
                {
                    currentState = state;
                    currentState.Start(updateData);
                    return;
                }
            }
            throw new Exception("AI STATE HANDLER ERROR: ChangeState request failed, no state by the name of " + newState + " found.");
        }
    }
}
