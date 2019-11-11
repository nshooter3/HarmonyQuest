namespace GameAI.StateHandlers
{
    using Behaviors;

    public class BasicEnemyStateHandler : AIStateHandler
    {
        public override void Init()
        {
            states = new AIBehavior[]{ new IdleBehavior { }, new NavigateBehavior { }, new EngageBehavior { }, new DisengageBehavior { } };
            currentState = states[0];
        }

        protected override void CheckForStateChange(AIStateUpdateData updateData)
        {
            //TODO: Make this change states and stuff.
        }
    }
}