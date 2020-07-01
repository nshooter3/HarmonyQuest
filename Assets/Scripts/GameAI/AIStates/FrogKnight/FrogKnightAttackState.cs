namespace GameAI.AIStates.FrogKnight
{
    using GameAI.AIStateActions;
    using GameAI.StateHandlers;
    using UnityEngine;

    public class FrogKnightAttackState : AIState
    {
        private DebugAction debugAction = new DebugAction();

        public override void Init(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.DebugChangeColor(Color.white);
            updateData.aiGameObjectFacade.ActivateHitbox("BasicAttack", 0f, updateData.player.GetConfig().PostCounterGracePeriod, 10);
            updateData.aiGameObjectFacade.attacking = false;
            updateData.animator.SetBool("AttackBool", true);
            Debug.Log("FrogKnightAttackState");
        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {
            //Update navpos graphic for debug. Shows where the agent is focusing.
            debugAction.NavPosTrackTarget(updateData);
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
            updateData.animator.SetBool("AttackBool", false);
        }
    }
}
