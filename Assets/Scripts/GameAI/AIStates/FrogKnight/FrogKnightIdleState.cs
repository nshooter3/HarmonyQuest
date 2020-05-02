namespace GameAI.AIStates.FrogKnight
{
    using GameAI.AIStateActions;
    using GameAI.Navigation;
    using GameAI.StateHandlers;
    using UnityEngine;

    public class FrogKnightIdleState : AIState
    {
        private bool aggroZoneEntered = false;

        private DebugAction debugAction = new DebugAction();

        public override void Init(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.data.isAggroed = false;
            updateData.aiGameObjectFacade.SetRigidBodyConstraintsToLockAllButGravity();
            if (updateData.aiGameObjectFacade.data.aggroZone != null)
            {
                updateData.aiGameObjectFacade.data.aggroZone.AssignFunctionToTriggerStayDelegate(AggroZoneActivation);
            }
        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {
            
        }

        public override void OnFixedUpdate(AIStateUpdateData updateData)
        {
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
            else if (aggroZoneEntered && !NavMeshUtil.IsTargetObstructed(updateData.aiGameObjectFacade.data.aiAgentBottom, updateData.player.GetTransform()))
            {
                updateData.stateHandler.RequestStateTransition(new FrogKnightAggroState { }, updateData);
            }
        }

        public override void Abort(AIStateUpdateData updateData)
        {
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
