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

        public void OnUpdate(AIStateUpdateData updateData)
        {
            CheckForStateChange(updateData);
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

        public abstract void AggroZoneActivation(Collider other);
    }
}
