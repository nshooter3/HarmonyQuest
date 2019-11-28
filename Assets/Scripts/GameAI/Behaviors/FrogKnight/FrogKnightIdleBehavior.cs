﻿namespace GameAI.Behaviors.FrogKnight
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
            updateData.agentComponentInterface.targetInLineOfSight = false;
            updateData.agentComponentInterface.SetRigidBodyConstraintsToLockAllButGravity();
        }

        public override void Update(AIStateUpdateData updateData)
        {
            updateData.agentComponentInterface.ApplyGravity();
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            aborted = true;
            readyForStateTransition = true;
        }
    }
}
