namespace GameAI.States.FrogKnight
{
    using GameAI.Navigation;
    using GameAI.StateHandlers;
    using UnityEngine;

    public class FrogKnightNavigateState : AIState
    {
        private float checkForTargetObstructionTimer = 0.0f;

        public override void Init(AIStateUpdateData updateData)
        {
            updateData.navigator.SetTarget(updateData.aiGameObjectFacade.data.aiAgentBottom, updateData.player.transform);
            updateData.aiGameObjectFacade.data.isAggroed = true;
            updateData.aiGameObjectFacade.SetRigidBodyConstraintsToDefault();
        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.data.navPos.transform.position = updateData.navigator.GetNextWaypoint();
            updateData.aiGameObjectFacade.SetVelocityTowardsDestination(updateData.navigator.GetNextWaypoint());
        }

        public override void OnFixedUpdate(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.ApplyVelocity();
            updateData.aiGameObjectFacade.ApplyGravity();
        }

        public override void OnBeatUpdate(AIStateUpdateData updateData)
        {
            
        }

        public override void CheckForStateChange(AIStateUpdateData updateData)
        {
            if (ShouldDeAggro(updateData) || updateData.navigator.isActivelyGeneratingPath == false)
            {
                checkForTargetObstructionTimer = 0;
                updateData.stateHandler.RequestStateTransition(new FrogKnightDisengageState { }, updateData);
            }
            else
            {

                checkForTargetObstructionTimer += Time.deltaTime;
                if (checkForTargetObstructionTimer > NavigatorSettings.checkForTargetObstructionRate)
                {
                    checkForTargetObstructionTimer = 0;
                    if (!NavMeshUtil.IsTargetObstructed(updateData.aiGameObjectFacade.data.aiAgentBottom, updateData.player.transform))
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
            return updateData.aiGameObjectFacade.data.disengageWithDistance && Vector3.Distance(updateData.aiGameObjectFacade.transform.position, updateData.player.transform.position) > updateData.aiGameObjectFacade.data.disengageDistance;
        }
    }
}
