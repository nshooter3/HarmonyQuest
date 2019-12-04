namespace GameAI.Behaviors.FrogKnight
{
    using GameAI.StateHandlers;
    using UnityEngine;

    public class FrogKnightIdleBehavior : AIBehavior
    {
        public override string GetName()
        {
            return "idle";
        }

        public override void Start(AIStateUpdateData updateData)
        {
            updateData.aiGameObject.targetInLineOfSight = false;
            updateData.aiGameObject.SetRigidBodyConstraintsToLockAllButGravity();
        }

        public override void Update(AIStateUpdateData updateData)
        {
            
        }

        public override void FixedUpdate(AIStateUpdateData updateData)
        {
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
