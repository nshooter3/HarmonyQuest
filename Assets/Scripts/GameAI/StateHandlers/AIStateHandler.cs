﻿namespace GameAI.StateHandlers
{
    using Behaviors;

    public abstract class AIStateHandler
    {
        protected AIBehavior currentState;
        protected AIBehavior nextState;

        public abstract void Init();
        protected abstract void CheckForStateChange(AIStateUpdateData updateData);

        public void Update(AIStateUpdateData updateData)
        {
            CheckForStateChange(updateData);
            if (nextState != null && currentState.readyForStateTransition)
            {
                currentState = nextState;
                nextState = null;
                currentState.Start(updateData);
            }
            else
            {
                currentState.Update(updateData);
            }
        }

        public AIBehavior GetCurrentState()
        {
            return currentState;
        }

        protected void RequestStateTransition(AIBehavior nextState, AIStateUpdateData updateData)
        {
            currentState.Abort(updateData);
            this.nextState = nextState;
        }
    }
}
