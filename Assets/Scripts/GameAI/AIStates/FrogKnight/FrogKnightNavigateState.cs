namespace GameAI.AIStates.FrogKnight
{
    using GameAI.AIStateActions;
    using GameAI.Navigation;
    using GameAI.StateHandlers;
    using UnityEngine;

    public class FrogKnightNavigateState : AIState
    {
        private float checkForTargetObstructionTimer = 0.0f;

        private DebugAction debugAction = new DebugAction();

        //How many beats we've been in this state.
        private int beatCount = 0;

        public override void Init(AIStateUpdateData updateData)
        {
            updateData.navigator.SetTarget(updateData.aiGameObjectFacade.data.aiAgentBottom, updateData.player.GetTransform());
            updateData.aiGameObjectFacade.data.isAggroed = true;
            //updateData.aiGameObjectFacade.hasAttackedSinceEngaging = false;
            updateData.aiGameObjectFacade.SetRigidBodyConstraintsToDefault();
        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {
            debugAction.NavPosSetPosition(updateData, updateData.navigator.GetNextWaypoint());
            updateData.aiGameObjectFacade.SetVelocityTowardsDestination(updateData.navigator.GetNextWaypoint());
        }

        public override void OnFixedUpdate(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.ApplyVelocity();
            updateData.aiGameObjectFacade.ApplyGravity();
        }

        public override void OnBeatUpdate(AIStateUpdateData updateData)
        {
            beatCount++;

            //If we've been in the navigate state for a while, attack ASAP upon reentering the engage state.
            if (beatCount >= 4 && updateData.aiGameObjectFacade.shouldAttackAsSoonAsPossible == false)
            {
                updateData.aiGameObjectFacade.shouldAttackAsSoonAsPossible = true;
            }
        }

        public override void CheckForStateChange(AIStateUpdateData updateData)
        {
            if (updateData.aiGameObjectFacade.IsDead() == true)
            {
                updateData.stateHandler.RequestStateTransition(new FrogKnightDeadState { }, updateData);
            }
            else if (ShouldDeAggro(updateData) || updateData.navigator.isActivelyGeneratingPath == false)
            {
                checkForTargetObstructionTimer = 0;
                updateData.stateHandler.RequestStateTransition(new FrogKnightLoseTargetState { }, updateData);
            }
            else
            {
                checkForTargetObstructionTimer += Time.deltaTime;
                if (checkForTargetObstructionTimer > NavigatorSettings.checkForTargetObstructionRate)
                {
                    checkForTargetObstructionTimer = 0;
                    if (!NavMeshUtil.IsTargetObstructed(updateData.aiGameObjectFacade.data.aiAgentBottom, updateData.player.GetTransform()))
                    {
                        updateData.stateHandler.RequestStateTransition(new FrogKnightEngageState { }, updateData);
                    }
                }
            }
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.navigator.CancelCurrentNavigation();
            updateData.aiGameObjectFacade.ResetVelocity();
            aborted = true;
            readyForStateTransition = true;
        }

        private bool ShouldDeAggro(AIStateUpdateData updateData)
        {
            return updateData.aiGameObjectFacade.data.aiStats.disengageWithDistance && Vector3.Distance(updateData.aiGameObjectFacade.transform.position, updateData.player.GetTransform().position) > updateData.aiGameObjectFacade.data.aiStats.disengageDistance;
        }
    }
}
