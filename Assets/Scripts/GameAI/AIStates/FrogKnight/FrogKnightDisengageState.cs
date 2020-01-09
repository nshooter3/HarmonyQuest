namespace GameAI.States.FrogKnight
{
    using GameAI.Navigation;
    using GameAI.StateHandlers;
    using UnityEngine;

    public class FrogKnightDisengageState : AIState
    {
        private bool aggroZoneEntered = false;

        public override void Init(AIStateUpdateData updateData)
        {
            updateData.navigator.SetTarget(updateData.aiGameObject.AIAgentBottom, updateData.aiGameObject.Origin);
            updateData.aiGameObject.isAggroed = false;
            updateData.aiGameObject.SetRigidBodyConstraintsToDefault();
            if (updateData.aiGameObject.AggroZone != null)
            {
                updateData.aiGameObject.AggroZone.AssignFunctionToTriggerStayDelegate(AggroZoneActivation);
            }
        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {
            updateData.aiGameObject.NavPos.transform.position = updateData.navigator.GetNextWaypoint();
            updateData.aiGameObject.SetVelocityTowardsDestination(updateData.navigator.GetNextWaypoint());
        }

        public override void OnFixedUpdate(AIStateUpdateData updateData)
        {
            updateData.aiGameObject.ApplyVelocity();
            updateData.aiGameObject.ApplyGravity();
        }

        public override void OnBeatUpdate(AIStateUpdateData updateData)
        {

        }

        public override void CheckForStateChange(AIStateUpdateData updateData)
        {
            if (aggroZoneEntered && !NavMeshUtil.IsTargetObstructed(updateData.aiGameObject.AIAgentBottom, updateData.player.transform))
            {
                updateData.stateHandler.RequestStateTransition(new FrogKnightPassiveEngageState { }, updateData);
            }
            else if (updateData.navigator.navigationTarget == null || Vector3.Distance(updateData.aiGameObject.AIAgentBottom.position, updateData.navigator.navigationTarget.position) <= NavigatorSettings.waypointReachedDistanceThreshold)
            {
                updateData.stateHandler.RequestStateTransition(new FrogKnightIdleState { }, updateData);
            }
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.aiGameObject.ResetVelocity();
            updateData.navigator.CancelCurrentNavigation();
            aborted = true;
            readyForStateTransition = true;
            if (updateData.aiGameObject.AggroZone != null)
            {
                updateData.aiGameObject.AggroZone.RemoveFunctionFromCollisionStayDelegate(AggroZoneActivation);
            }
        }

        public void AggroZoneActivation(Collider other)
        {
            aggroZoneEntered = true;
        }
    }
}
