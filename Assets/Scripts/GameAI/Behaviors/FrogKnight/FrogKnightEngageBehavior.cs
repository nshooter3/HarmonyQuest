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
            Vector3 newNavPos = updateData.agentComponentInterface.aggroTarget.position;
            newNavPos.y += updateData.agentComponentInterface.navPosHeightOffset;

            updateData.agentComponentInterface.navPos.transform.position = newNavPos;
            updateData.agentComponentInterface.Move(updateData.agentComponentInterface.aggroTarget.position);
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            aborted = true;
            readyForStateTransition = true;
        }
    }
}
