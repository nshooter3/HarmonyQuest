namespace GameAI.AIStates.FrogKnight
{
    using GameAI.AIStateActions;
    using GameAI.StateHandlers;
    using UnityEngine;

    public class FrogKnightAttackCooldownState : AIState
    {
        private DebugAction debugAction = new DebugAction();

        public override void Init(AIStateUpdateData updateData)
        {
            updateData.animator.SetBool("AttackEndBool", true);
            //Debug.Log("FrogKnightAttackCooldownState");
        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {
            //Update navpos graphic for debug. Shows where the agent is focusing.
            debugAction.NavPosTrackTarget(updateData);
            updateData.aiGameObjectFacade.SetVelocity(Vector3.zero);
        }

        public override void OnFixedUpdate(AIStateUpdateData updateData)
        {
            if (updateData.aiGameObjectFacade.IsGrounded() && !updateData.aiGameObjectFacade.IsSliding())
            {
                updateData.aiGameObjectFacade.IgnoreHorizontalMovementInput();
                updateData.aiGameObjectFacade.ApplyVelocity();
            }
            updateData.aiGameObjectFacade.ApplyGravity(updateData.aiGameObjectFacade.data.aiStats.gravity, true);
        }

        public override void OnBeatUpdate(AIStateUpdateData updateData)
        {
            updateData.stateHandler.RequestStateTransition(new FrogKnightEngageState { }, updateData);
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
            updateData.aiGameObjectFacade.DebugChangeColor(Color.white);
            aborted = true;
            readyForStateTransition = true;
            updateData.animator.SetBool("AttackEndBool", false);
        }
    }
}
