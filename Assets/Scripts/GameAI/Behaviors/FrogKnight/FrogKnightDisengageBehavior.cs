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
            updateData.navigator.SetTarget(updateData.agentComponentInterface.aiAgentBottom, updateData.agentComponentInterface.origin);
            updateData.agentComponentInterface.targetInLineOfSight = false;
            updateData.agentComponentInterface.SetRigidBodyConstraintsToDefault();
        }

        public override void Update(AIStateUpdateData updateData)
        {
            updateData.agentComponentInterface.navPos.transform.position = updateData.navigator.GetNextWaypoint();
            updateData.agentComponentInterface.Move(updateData.navigator.GetNextWaypoint());
        }

        public override void FixedUpdate(AIStateUpdateData updateData)
        {
            updateData.agentComponentInterface.ApplyGravity();
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.navigator.CancelCurrentNavigation();
            aborted = true;
            readyForStateTransition = true;
        }
    }
}
