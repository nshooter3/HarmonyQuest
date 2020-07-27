namespace GameAI.AIStates.FrogKnight
{
    using GameAI.AIStateActions;
    using GameAI.Navigation;
    using GameAI.StateHandlers;
    using UnityEngine;

    public class FrogKnightIdleState : AIState
    {
        private bool aggroZoneEntered = false;

        private IdleWanderAction idleWanderAction;
        private DebugAction debugAction = new DebugAction();

        public override void Init(AIStateUpdateData updateData)
        {
            idleWanderAction = new IdleWanderAction(updateData, 5.0f, 2.0f, 5f);
            updateData.aiGameObjectFacade.data.isAggroed = false;
            updateData.aiGameObjectFacade.shouldAttackAsSoonAsPossible = true;
            updateData.aiGameObjectFacade.SetRigidBodyConstraintsToLockAllButGravity();
            if (updateData.aiGameObjectFacade.data.aggroZone != null)
            {
                updateData.aiGameObjectFacade.data.aggroZone.AssignFunctionToTriggerStayDelegate(AggroZoneActivation);
            }
        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {
            idleWanderAction.OnUpdate(updateData);
            if (idleWanderAction.IsWandering())
            {
                updateData.aiGameObjectFacade.SetRigidBodyConstraintsToDefault();
                updateData.aiGameObjectFacade.SetVelocityTowardsDestination(updateData.navigator.GetNextWaypoint(), true, 0.5f);
                debugAction.NavPosSetPosition(updateData, updateData.navigator.GetNextWaypoint());
            }
            else
            {
                updateData.aiGameObjectFacade.SetRigidBodyConstraintsToLockAllButGravity();
                updateData.aiGameObjectFacade.SetVelocity(Vector3.zero);
                debugAction.NavPosSetPosition(updateData, updateData.aiGameObjectFacade.data.aiAgentBottom.position);
            }
        }

        public override void OnFixedUpdate(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.ApplyVelocity(true, true, 1, true);
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
            else if (updateData.aiGameObjectFacade.TookDamageFromPlayerThisFrame() || 
                (aggroZoneEntered && !NavMeshUtil.IsTargetObstructed(updateData.aiGameObjectFacade.data.aiAgentBottom, updateData.player.GetTransform())))
            {
                updateData.stateHandler.RequestStateTransition(new FrogKnightAggroState { }, updateData);
            }
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.navigator.CancelCurrentNavigation();
            updateData.aiGameObjectFacade.ResetVelocity();
            aborted = true;
            readyForStateTransition = true;
            if (updateData.aiGameObjectFacade.data.aggroZone != null)
            {
                updateData.aiGameObjectFacade.data.aggroZone.RemoveFunctionFromTriggerStayDelegate(AggroZoneActivation);
            }
        }

        public void AggroZoneActivation(Collider other)
        {
            aggroZoneEntered = true;
        }
    }
}
