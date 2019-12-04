namespace GameAI.Behaviors
{
    using GameAI.StateHandlers;

    public abstract class AIBehavior
    {
        public bool readyForStateTransition = false;
        public bool aborted = false;
        public abstract string GetName();
        public abstract void Init(AIStateUpdateData updateData);
        public abstract void OnUpdate(AIStateUpdateData updateData);
        public abstract void OnFixedUpdate(AIStateUpdateData updateData);
        public abstract void Abort(AIStateUpdateData updateData);
    }
}
