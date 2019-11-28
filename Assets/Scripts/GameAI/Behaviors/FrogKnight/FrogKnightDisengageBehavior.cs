namespace GameAI.Behaviors.FrogKnight
{
    using GameAI.StateHandlers;

    public class FrogKnightDisengageBehavior : AIBehavior
    {
        public override string GetName()
        {
            return "disengage";
        }

        public override void Start(AIStateUpdateData updateData)
        {
            updateData.navigator.SetTarget(updateData.agentComponentInterface.AIAgentBottom, updateData.agentComponentInterface.Origin);
            updateData.agentComponentInterface.targetInLineOfSight = false;
            updateData.agentComponentInterface.SetRigidBodyConstraintsToDefault();
        }

        public override void Update(AIStateUpdateData updateData)
        {
            updateData.agentComponentInterface.NavPos.transform.position = updateData.navigator.GetNextWaypoint();
            updateData.agentComponentInterface.SetVelocity(updateData.navigator.GetNextWaypoint());
        }

        public override void FixedUpdate(AIStateUpdateData updateData)
        {
            updateData.agentComponentInterface.ApplyVelocity();
            updateData.agentComponentInterface.ApplyGravity();
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.agentComponentInterface.ResetVelocity();
            updateData.navigator.CancelCurrentNavigation();
            aborted = true;
            readyForStateTransition = true;
        }
    }
}
