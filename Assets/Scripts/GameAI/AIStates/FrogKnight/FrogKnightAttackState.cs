﻿namespace GameAI.AIStates.FrogKnight
{
    using GameAI.StateHandlers;
    using UnityEngine;

    public class FrogKnightAttackState : AIState
    {
        public override void Init(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.DebugChangeColor(Color.red);
            updateData.aiGameObjectFacade.ActivateHitbox("BasicAttack", 0f, 0.15f, 10);
        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {

        }

        public override void OnFixedUpdate(AIStateUpdateData updateData)
        {
            //updateData.aiGameObjectFacade.ApplyVelocity();
            //updateData.aiGameObjectFacade.ApplyGravity();
        }

        public override void OnBeatUpdate(AIStateUpdateData updateData)
        {
            updateData.stateHandler.RequestStateTransition(new FrogKnightAttackCooldownState { }, updateData);
        }

        public override void CheckForStateChange(AIStateUpdateData updateData)
        {
            if (updateData.aiGameObjectFacade.IsDead() == true)
            {
                updateData.stateHandler.RequestStateTransition(new FrogKnightDeadState { }, updateData);
            }
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.CancelHitbox("BasicAttack");
            aborted = true;
            readyForStateTransition = true;
        }
    }
}
