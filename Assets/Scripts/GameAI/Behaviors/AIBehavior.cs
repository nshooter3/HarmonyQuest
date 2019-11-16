namespace GameAI.Behaviors
{
    using GameAI.StateHandlers;

    public abstract class AIBehavior
    {
        public bool readyForStateTransition = false;
        public bool aborted = false;
        public abstract string GetName();
        public abstract void Start(AIStateUpdateData updateData);
        public abstract void Update(AIStateUpdateData updateData);
        public abstract void Abort(AIStateUpdateData updateData);
    }
}
