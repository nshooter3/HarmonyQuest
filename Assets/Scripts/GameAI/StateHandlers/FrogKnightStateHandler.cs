namespace GameAI.StateHandlers
{
    using Behaviors.FrogKnight;
    using GameAI.Navigation;
    using UnityEngine;

    public class FrogKnightStateHandler : AIStateHandler
    {
        private float checkForTargetObstructionTimer = 0.0f;
        private bool aggroZoneEntered = false;

        public override void Init(AIStateUpdateData updateData)
        {
            currentState = new FrogKnightIdleBehavior { };
            currentState.Init(updateData);
        }

        protected override void CheckForStateChange(AIStateUpdateData updateData)
        {
            switch (currentState.GetName())
            {
                case "idle":
                    CheckForIdleStateChange(updateData);
                    break;
                case "engage":
                    CheckForEngageStateChange(updateData);
                    break;
                case "navigate":
                    CheckForNavigateStateChange(updateData);
                    break;
                case "disengage":
                    CheckForDisengageStateChange(updateData);
                    break;
            }
            aggroZoneEntered = false;
        }

        private void CheckForIdleStateChange(AIStateUpdateData updateData)
        {
            if (aggroZoneEntered && !NavMeshUtil.IsTargetObstructed(updateData.aiGameObject.AIAgentBottom, updateData.player.transform))
            {
                RequestStateTransition(new FrogKnightEngageBehavior { }, updateData);
            }
        }

        private void CheckForEngageStateChange(AIStateUpdateData updateData)
        {
            if (ShouldDeAggro(updateData))
            {
                checkForTargetObstructionTimer = 0;
                RequestStateTransition(new FrogKnightDisengageBehavior { }, updateData);
            }
            else
            {
                checkForTargetObstructionTimer += Time.deltaTime;
                if (checkForTargetObstructionTimer > NavigatorSettings.checkForTargetObstructionRate)
                {
                    checkForTargetObstructionTimer = 0;
                    if (NavMeshUtil.IsTargetObstructed(updateData.aiGameObject.AIAgentBottom, updateData.player.transform))
                    {
                        RequestStateTransition(new FrogKnightNavigateBehavior { }, updateData);
                    }
                }
            }
        }

        private void CheckForNavigateStateChange(AIStateUpdateData updateData)
        {
            if (ShouldDeAggro(updateData) || updateData.navigator.isActivelyGeneratingPath == false)
            {
                checkForTargetObstructionTimer = 0;
                RequestStateTransition(new FrogKnightDisengageBehavior { }, updateData);
            }
            else
            {

                checkForTargetObstructionTimer += Time.deltaTime;
                if (checkForTargetObstructionTimer > NavigatorSettings.checkForTargetObstructionRate)
                {
                    checkForTargetObstructionTimer = 0;
                    if (!NavMeshUtil.IsTargetObstructed(updateData.aiGameObject.AIAgentBottom, updateData.player.transform))
                    {
                        RequestStateTransition(new FrogKnightEngageBehavior { }, updateData);
                    }
                }
            }
        }

        private void CheckForDisengageStateChange(AIStateUpdateData updateData)
        {
            if (aggroZoneEntered && !NavMeshUtil.IsTargetObstructed(updateData.aiGameObject.AIAgentBottom, updateData.player.transform))
            {
                RequestStateTransition(new FrogKnightEngageBehavior { }, updateData);
            }
            else if (Vector3.Distance(updateData.aiGameObject.AIAgentBottom.position, updateData.navigator.navigationTarget.position) <= NavigatorSettings.waypointReachedDistanceThreshold)
            {
                RequestStateTransition(new FrogKnightIdleBehavior { }, updateData);
            }
        }

        private bool ShouldDeAggro(AIStateUpdateData updateData)
        {
            return updateData.aiGameObject.DisengageWithDistance && Vector3.Distance(updateData.aiGameObject.transform.position, updateData.player.transform.position) > updateData.aiGameObject.DisengageDistance;
        }

        public override void AggroZoneActivation(Collider other)
        {
            aggroZoneEntered = true;
        }
    }
}