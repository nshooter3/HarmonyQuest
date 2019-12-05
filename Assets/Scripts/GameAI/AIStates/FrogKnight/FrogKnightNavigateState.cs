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
            updateData.navigator.SetTarget(updateData.aiGameObject.AIAgentBottom, updateData.player.transform);
            updateData.aiGameObject.targetInLineOfSight = true;
            updateData.aiGameObject.SetRigidBodyConstraintsToDefault();
        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {
            updateData.aiGameObject.NavPos.transform.position = updateData.navigator.GetNextWaypoint();
            updateData.aiGameObject.SetVelocity(updateData.navigator.GetNextWaypoint());
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
                    if (!NavMeshUtil.IsTargetObstructed(updateData.aiGameObject.AIAgentBottom, updateData.player.transform))
                    {
                        updateData.stateHandler.RequestStateTransition(new FrogKnightEngageState { }, updateData);
                    }
                }
            }
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.navigator.CancelCurrentNavigation();
            updateData.aiGameObject.ResetVelocity();
            aborted = true;
            readyForStateTransition = true;
        }

        private bool ShouldDeAggro(AIStateUpdateData updateData)
        {
            return updateData.aiGameObject.DisengageWithDistance && Vector3.Distance(updateData.aiGameObject.transform.position, updateData.player.transform.position) > updateData.aiGameObject.DisengageDistance;
        }
    }
}
