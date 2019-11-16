namespace GameAI.StateHandlers
{
    using Behaviors;

    public class BasicEnemyStateHandler : AIStateHandler
    {
        public override void Init()
        {
            currentState = new IdleBehavior { };
        }

        protected override void CheckForStateChange(AIStateUpdateData updateData)
        {
            //TODO: Make this change states and stuff.
        }
    }
}