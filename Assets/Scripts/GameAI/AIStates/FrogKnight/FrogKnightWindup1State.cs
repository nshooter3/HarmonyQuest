namespace GameAI.AIStates.FrogKnight
{
    using GameAI.AIStateActions;
    using GameAI.StateHandlers;
    using UnityEngine;

    public class FrogKnightWindup1State : AIState
    {
        private MoveAction moveAction = new MoveAction();

        //The distance at which we are close enough, and stop trying to approach the target.
        float attackApproachCutoffRange = 4.0f;

        private bool willMoveIntoRangeThisFrame = false;

        public override void Init(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.DebugChangeColor(Color.yellow);
        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {
            if (moveAction.SeekDestinationIfOutOfRange(updateData.aiGameObjectFacade, updateData.aiGameObjectFacade.data.aggroTarget.position, attackApproachCutoffRange, true, 0.5f, true))
            {
                updateData.aiGameObjectFacade.SetRigidBodyConstraintsToDefault();
                willMoveIntoRangeThisFrame = true;
            }
            else
            {
                updateData.aiGameObjectFacade.SetVelocity(Vector3.zero);
                updateData.aiGameObjectFacade.SetRigidBodyConstraintsToLockAllButGravity();
                willMoveIntoRangeThisFrame = false;
                updateData.aiGameObjectFacade.SetRotationDirection(true);
            }
        }

        public override void OnFixedUpdate(AIStateUpdateData updateData)
        {
            if (willMoveIntoRangeThisFrame)
            {
                updateData.aiGameObjectFacade.ApplyVelocity(true, true, 0.5f);
            }
            else
            {
                updateData.aiGameObjectFacade.Rotate(updateData.aiGameObjectFacade.GetRotationDirection(), 0.5f);
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
        }
    }
}
