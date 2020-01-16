namespace GameAI.States.FrogKnight
{
    using GameAI.Navigation;
    using GameAI.StateHandlers;
    using UnityEngine;

    public class FrogKnightIdleState : AIState
    {
        private bool aggroZoneEntered = false;

        public override void Init(AIStateUpdateData updateData)
        {
            updateData.aiGameObject.isAggroed = false;
            updateData.aiGameObject.SetRigidBodyConstraintsToLockAllButGravity();
            if (updateData.aiGameObject.AggroZone != null)
            {
                updateData.aiGameObject.AggroZone.AssignFunctionToTriggerStayDelegate(AggroZoneActivation);
            }
        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {
            
        }

        public override void OnFixedUpdate(AIStateUpdateData updateData)
        {
            updateData.aiGameObject.ApplyGravity();
        }

        public override void OnBeatUpdate(AIStateUpdateData updateData)
        {

        }

        public override void CheckForStateChange(AIStateUpdateData updateData)
        {
            if (aggroZoneEntered && !NavMeshUtil.IsTargetObstructed(updateData.aiGameObject.AIAgentBottom, updateData.player.transform))
            {
                updateData.stateHandler.RequestStateTransition(new FrogKnightEngageState { }, updateData);
            }
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.aiGameObject.ResetVelocity();
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
