namespace GameAI.AIStates.FrogKnight
{
    using GameAI.AIStateActions;
    using GameAI.Navigation;
    using GameAI.StateHandlers;
    using UnityEngine;

    public class FrogKnightDisengageState : AIState
    {
        private bool aggroZoneEntered = false;

        private DebugAction debugAction = new DebugAction();

        public override void Init(AIStateUpdateData updateData)
        {
            updateData.navigator.SetTarget(updateData.aiGameObjectFacade.data.aiAgentBottom, updateData.aiGameObjectFacade.data.origin);
            updateData.aiGameObjectFacade.data.isAggroed = false;
            updateData.aiGameObjectFacade.SetRigidBodyConstraintsToDefault();
            if (updateData.aiGameObjectFacade.data.aggroZone != null)
            {
                updateData.aiGameObjectFacade.data.aggroZone.AssignFunctionToTriggerStayDelegate(AggroZoneActivation);
            }
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

        }

        public override void CheckForStateChange(AIStateUpdateData updateData)
        {
            if (updateData.aiGameObjectFacade.IsDead() == true)
            {
                updateData.stateHandler.RequestStateTransition(new FrogKnightDeadState { }, updateData);
            }
            else if (aggroZoneEntered && !NavMeshUtil.IsTargetObstructed(updateData.aiGameObjectFacade.data.aiAgentBottom, updateData.player.transform))
            {
                updateData.stateHandler.RequestStateTransition(new FrogKnightEngageState { }, updateData);
            }
            else if (updateData.navigator.navigationTarget == null || Vector3.Distance(updateData.aiGameObjectFacade.data.aiAgentBottom.position, updateData.navigator.navigationTarget.position) <= NavigatorSettings.waypointReachedDistanceThreshold)
            {
                updateData.stateHandler.RequestStateTransition(new FrogKnightIdleState { }, updateData);
            }
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.ResetVelocity();
            updateData.navigator.CancelCurrentNavigation();
            aborted = true;
            readyForStateTransition = true;
            if (updateData.aiGameObjectFacade.data.aggroZone != null)
            {
                updateData.aiGameObjectFacade.data.aggroZone.RemoveFunctionFromCollisionStayDelegate(AggroZoneActivation);
            }
        }

        public void AggroZoneActivation(Collider other)
        {
            aggroZoneEntered = true;
        }
    }
}
