namespace GameAI.Behaviors.FrogKnight
{
    using GameAI.StateHandlers;

    public class FrogKnightNavigateBehavior : AIBehavior
    {
        public override string GetName()
        {
            return "navigate";
        }

        public override void Start(AIStateUpdateData updateData)
        {
            updateData.navigator.SetTarget(updateData.agentComponentInterface.aiAgentBottom, updateData.player.transform);
            updateData.agentComponentInterface.targetInLineOfSight = true;
            updateData.agentComponentInterface.SetRigidBodyConstraintsToDefault();
        }

        public override void Update(AIStateUpdateData updateData)
        {
            updateData.agentComponentInterface.navPos.transform.position = updateData.navigator.GetNextWaypoint();
            updateData.agentComponentInterface.SetVelocity(updateData.navigator.GetNextWaypoint());
        }

        public override void FixedUpdate(AIStateUpdateData updateData)
        {
            updateData.agentComponentInterface.ApplyVelocity();
            updateData.agentComponentInterface.ApplyGravity();
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.navigator.CancelCurrentNavigation();
            updateData.agentComponentInterface.ResetVelocity();
            aborted = true;
            readyForStateTransition = true;
        }
    }
}
