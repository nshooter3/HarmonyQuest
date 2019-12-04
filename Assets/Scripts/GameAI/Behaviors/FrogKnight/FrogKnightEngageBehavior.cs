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
            updateData.aiGameObject.targetInLineOfSight = true;
            updateData.aiGameObject.SetRigidBodyConstraintsToDefault();
        }

        public override void Update(AIStateUpdateData updateData)
        {
            Vector3 newNavPos = updateData.aiGameObject.AggroTarget.position;
            newNavPos.y += updateData.aiGameObject.NavPosHeightOffset;

            updateData.aiGameObject.NavPos.transform.position = newNavPos;
            updateData.aiGameObject.SetVelocity(updateData.aiGameObject.AggroTarget.position);
        }

        public override void FixedUpdate(AIStateUpdateData updateData)
        {
            updateData.aiGameObject.ApplyVelocity();
            updateData.aiGameObject.ApplyGravity();
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.aiGameObject.ResetVelocity();
            aborted = true;
            readyForStateTransition = true;
        }
    }
}
