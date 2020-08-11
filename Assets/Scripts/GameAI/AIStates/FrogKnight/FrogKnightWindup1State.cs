namespace GameAI.AIStates.FrogKnight
{
    using GameAI.AIStateActions;
    using GameAI.StateHandlers;
    using HarmonyQuest;
    using UnityEngine;

    public class FrogKnightWindup1State : AIState
    {
        private MoveAction moveAction = new MoveAction();
        private DebugAction debugAction = new DebugAction();

        //The distance at which we are close enough, and stop trying to approach the target.
        float attackApproachCutoffRange = 4.0f;

        private bool inAttackRange = false;

        public override void Init(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.DebugChangeColor(Color.yellow);
            updateData.aiGameObjectFacade.SetRigidBodyConstraintsToDefault();
            updateData.animator.SetBool("AttackStartBool", true);
            ServiceLocator.instance.GetAIAgentManager().SetNextAttackMinimumCooldownBeats(2);
            //Debug.Log("FrogKnightWindup1State");
        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {
            //Update navpos graphic for debug. Shows where the agent is focusing.
            debugAction.NavPosTrackTarget(updateData);

            if (updateData.aiGameObjectFacade.GetDistanceFromAggroTarget() > attackApproachCutoffRange)
            {
                moveAction.SeekDestination(updateData.aiGameObjectFacade, updateData.aiGameObjectFacade.data.aggroTarget.position, true, 0.5f, true);
                if (inAttackRange == true)
                {
                    updateData.aiGameObjectFacade.SetRigidBodyConstraintsToDefault();
                    inAttackRange = false;
                }
            }
            else
            {
                if (inAttackRange == false)
                {
                    updateData.aiGameObjectFacade.SetVelocity(Vector3.zero);
                    updateData.aiGameObjectFacade.SetRigidBodyConstraintsToLockAllButGravity();
                    inAttackRange = true;
                }
            }
        }

        public override void OnFixedUpdate(AIStateUpdateData updateData)
        {
            if (inAttackRange == false)
            {
                updateData.aiGameObjectFacade.ApplyVelocity(true, true, 0.5f);
            }
            else
            {
                updateData.aiGameObjectFacade.Rotate(updateData.aiGameObjectFacade.data.aiStats.rotateSpeed * 0.5f);
            }
            updateData.aiGameObjectFacade.ApplyGravity();
        }

        public override void OnBeatUpdate(AIStateUpdateData updateData)
        {
            updateData.stateHandler.RequestStateTransition(new FrogKnightWindup2State { }, updateData);
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
            aborted = true;
            readyForStateTransition = true;
            updateData.aiGameObjectFacade.SetVelocity(Vector3.zero);
            updateData.aiGameObjectFacade.SetRigidBodyConstraintsToLockAllButGravity();
            updateData.animator.SetBool("AttackStartBool", false);
        }
    }
}
