namespace GameAI.Behaviors.FrogKnight
{
    using GameAI.StateHandlers;
    using UnityEngine;

    public class FrogKnightEngageBehavior : AIBehavior
    {
        public override string GetName()
        {
            return "engage";
        }

        public override void Start(AIStateUpdateData updateData)
        {
            updateData.agentComponentInterface.targetInLineOfSight = true;
            updateData.agentComponentInterface.SetRigidBodyConstraintsToDefault();
        }

        public override void Update(AIStateUpdateData updateData)
        {
            Vector3 newNavPos = updateData.agentComponentInterface.AggroTarget.position;
            newNavPos.y += updateData.agentComponentInterface.NavPosHeightOffset;

            updateData.agentComponentInterface.NavPos.transform.position = newNavPos;
            updateData.agentComponentInterface.SetVelocity(updateData.agentComponentInterface.AggroTarget.position);
        }

        public override void FixedUpdate(AIStateUpdateData updateData)
        {
            updateData.agentComponentInterface.ApplyVelocity();
            updateData.agentComponentInterface.ApplyGravity();
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.agentComponentInterface.ResetVelocity();
            aborted = true;
            readyForStateTransition = true;
        }
    }
}
