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
            updateData.navigator.SetTarget(updateData.aiGameObject.AIAgentBottom, updateData.aiGameObject.Origin);
            updateData.aiGameObject.targetInLineOfSight = false;
            updateData.aiGameObject.SetRigidBodyConstraintsToDefault();
        }

        public override void Update(AIStateUpdateData updateData)
        {
            updateData.aiGameObject.NavPos.transform.position = updateData.navigator.GetNextWaypoint();
            updateData.aiGameObject.SetVelocity(updateData.navigator.GetNextWaypoint());
        }

        public override void FixedUpdate(AIStateUpdateData updateData)
        {
            updateData.aiGameObject.ApplyVelocity();
            updateData.aiGameObject.ApplyGravity();
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.aiGameObject.ResetVelocity();
            updateData.navigator.CancelCurrentNavigation();
            aborted = true;
            readyForStateTransition = true;
        }
    }
}
