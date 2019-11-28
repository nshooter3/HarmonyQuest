namespace GameAI.StateHandlers
{
    using Behaviors;
    using UnityEngine;

    public abstract class AIStateHandler
    {
        protected AIBehavior currentState;
        protected AIBehavior nextState;

        public abstract void Init(AIStateUpdateData updateData);
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

        public void FixedUpdate(AIStateUpdateData updateData)
        {
            currentState.FixedUpdate(updateData);
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

        public abstract void AggroZoneActivation(Collider other);
    }
}
