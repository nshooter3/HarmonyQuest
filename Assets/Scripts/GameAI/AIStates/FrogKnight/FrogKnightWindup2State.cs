namespace GameAI.AIStates.FrogKnight
{
    using GameAI.AIStateActions;
    using GameAI.StateHandlers;
    using HarmonyQuest.Audio;
    using UnityEngine;

    public class FrogKnightWindup2State : AIState
    {
        private MoveAction moveAction = new MoveAction();
        private DebugAction debugAction = new DebugAction();

        //The distance at which we are close enough, and stop trying to approach the target when flying at them.
        float attackSnapCutoffRange = 3.0f;

        private bool inAttackRange = false;

        float attackSnapWaitTime = FmodMusicHandler.instance.GetBeatDuration() * 0.65f;

        public override void Init(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.DebugChangeColor(new Color(1f, 0.5f, 0f));
            updateData.aiGameObjectFacade.SetRigidBodyConstraintsToDefault();
            updateData.animator.SetBool("AttackHoldBool", true);
            //Debug.Log("FrogKnightWindup2State");
        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {
            //Update navpos graphic for debug. Shows where the agent is focusing.
            debugAction.NavPosTrackTarget(updateData);

            if (attackSnapWaitTime > 0.0f)
            {
                attackSnapWaitTime -= Time.deltaTime;
            }
            else
            {
                if (updateData.aiGameObjectFacade.GetDistanceFromAggroTarget() > attackSnapCutoffRange)
                {
                    moveAction.SeekDestination(updateData.aiGameObjectFacade, updateData.aiGameObjectFacade.data.aggroTarget.position, true, 2.0f, true);
                    if (inAttackRange == true)
                    {
                        inAttackRange = false;
                        updateData.aiGameObjectFacade.SetRigidBodyConstraintsToDefault();
                    }
                }
                else
                {
                    updateData.aiGameObjectFacade.SetVelocity(Vector3.zero);
                    if (inAttackRange == false)
                    {
                        inAttackRange = true;
                        updateData.aiGameObjectFacade.SetRigidBodyConstraintsToLockAllButGravity();
                    }
                }
            }
        }

        public override void OnFixedUpdate(AIStateUpdateData updateData)
        {
            if (attackSnapWaitTime > 0.0f)
            {
                updateData.aiGameObjectFacade.Rotate(updateData.aiGameObjectFacade.data.aiStats.rotateSpeed * 0.15f);
            }
            else
            {
                updateData.aiGameObjectFacade.ApplyVelocity(true, true, 0.35f);
            }
            updateData.aiGameObjectFacade.ApplyGravity(updateData.aiGameObjectFacade.data.aiStats.gravity);
        }

        public override void OnBeatUpdate(AIStateUpdateData updateData)
        {
            updateData.stateHandler.RequestStateTransition(new FrogKnightAttackState { }, updateData);
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
            updateData.animator.SetBool("AttackHoldBool", false);
        }
    }
}
