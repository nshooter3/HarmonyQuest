﻿namespace GameAI.Behaviors.FrogKnight
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
            updateData.agentComponentInterface.SetRigidbodyConstraints(updateData.agentComponentInterface.defaultConstraints);
        }

        public override void Update(AIStateUpdateData updateData)
        {
            updateData.agentComponentInterface.navPos.transform.position = updateData.navigator.GetNextWaypoint();
            updateData.agentComponentInterface.Move(updateData.navigator.GetNextWaypoint());
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.navigator.CancelCurrentNavigation();
            aborted = true;
            readyForStateTransition = true;
        }
    }
}
