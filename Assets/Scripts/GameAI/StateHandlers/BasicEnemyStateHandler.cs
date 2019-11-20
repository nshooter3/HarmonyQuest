namespace GameAI.StateHandlers
{
    using Behaviors;
    using GameAI.Navigation;
    using UnityEngine;

    public class BasicEnemyStateHandler : AIStateHandler
    {
        private float checkForTargetObstructionTimer = 0.0f;
        private bool aggroZoneEntered = false;

        public override void Init(AIStateUpdateData updateData)
        {
            currentState = new IdleBehavior { };
            currentState.Start(updateData);
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
            if (aggroZoneEntered && !NavMeshUtil.IsTargetObstructed(updateData.agent.aiAgentBottom, updateData.player.transform))
            {
                updateData.agent.stateHandler.RequestStateTransition(new EngageBehavior { }, updateData);
            }
        }

        private void CheckForEngageStateChange(AIStateUpdateData updateData)
        {
            if (ShouldDeAggro(updateData))
            {
                checkForTargetObstructionTimer = 0;
                updateData.agent.stateHandler.RequestStateTransition(new DisengageBehavior { }, updateData);
            }
            else
            {
                checkForTargetObstructionTimer += Time.deltaTime;
                if (checkForTargetObstructionTimer > NavigatorSettings.checkForTargetObstructionRate)
                {
                    checkForTargetObstructionTimer = 0;
                    if (NavMeshUtil.IsTargetObstructed(updateData.agent.aiAgentBottom, updateData.player.transform))
                    {
                        updateData.agent.stateHandler.RequestStateTransition(new NavigateBehavior { }, updateData);
                    }
                }
            }
        }

        private void CheckForNavigateStateChange(AIStateUpdateData updateData)
        {
            if (ShouldDeAggro(updateData) || updateData.navigator.isActivelyGeneratingPath == false)
            {
                checkForTargetObstructionTimer = 0;
                updateData.agent.stateHandler.RequestStateTransition(new DisengageBehavior { }, updateData);
            }
            else
            {

                checkForTargetObstructionTimer += Time.deltaTime;
                if (checkForTargetObstructionTimer > NavigatorSettings.checkForTargetObstructionRate)
                {
                    checkForTargetObstructionTimer = 0;
                    if (!NavMeshUtil.IsTargetObstructed(updateData.agent.aiAgentBottom, updateData.player.transform))
                    {
                        updateData.agent.stateHandler.RequestStateTransition(new EngageBehavior { }, updateData);
                    }
                }
            }
        }

        private void CheckForDisengageStateChange(AIStateUpdateData updateData)
        {
            if (aggroZoneEntered && !NavMeshUtil.IsTargetObstructed(updateData.agent.aiAgentBottom, updateData.player.transform))
            {
                updateData.agent.stateHandler.RequestStateTransition(new EngageBehavior { }, updateData);
            }
            else if (Vector3.Distance(updateData.agent.aiAgentBottom.position, updateData.agent.navigator.navigationTarget.position) <= NavigatorSettings.waypointReachedDistanceThreshold)
            {
                updateData.agent.stateHandler.RequestStateTransition(new IdleBehavior { }, updateData);
            }
        }

        private bool ShouldDeAggro(AIStateUpdateData updateData)
        {
            return updateData.agent.disengageWithDistance && Vector3.Distance(updateData.agent.transform.position, updateData.player.transform.position) > updateData.agent.disengageDistance;
        }

        public override void AggroZoneActivation(Collider other)
        {
            aggroZoneEntered = true;
        }
    }
}