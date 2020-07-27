namespace GameAI.AIStateActions
{
    using GameAI.AIGameObjects;
    using GameAI.Navigation;
    using GameAI.StateHandlers;
    using UnityEngine;
    using UnityEngine.AI;

    public class IdleWanderAction
    {
        //Determines what kind of wander action this enemy will perform.
        public enum WanderAction { Wait, Wander };
        public WanderAction wanderAction = WanderAction.Wait;

        float curActionTimer = 0f;
        float maxWaitTime, minWaitTime;

        StrafeHitboxes strafeHitboxes;

        NavMeshHit hit;

        Transform agentTransform;
        Transform origin;

        Transform wanderNavigationTarget;

        float wanderRadius;

        Vector3 randomWanderDestination;

        public IdleWanderAction(AIStateUpdateData updateData, float maxWaitTime = 4.0f, float minWaitTime = 1.0f, float wanderRadius = 10f)
        {
            this.maxWaitTime = maxWaitTime;
            this.minWaitTime = minWaitTime;
            strafeHitboxes = updateData.aiGameObjectFacade.data.strafeHitBoxes;
            curActionTimer = Random.Range(this.maxWaitTime, this.minWaitTime);
            this.wanderRadius = wanderRadius;
            origin = updateData.aiGameObjectFacade.data.origin;
            agentTransform = updateData.aiGameObjectFacade.transform;
            wanderNavigationTarget = updateData.aiGameObjectFacade.data.wanderNavigationTarget;
            wanderNavigationTarget.parent = null;
        }

        public void OnUpdate(AIStateUpdateData updateData)
        {
            switch (wanderAction) {
                case WanderAction.Wait:
                    WaitUpdate(updateData);
                    break;
                case WanderAction.Wander:
                    WanderUpdate(updateData);
                    break;
            }
        }

        private void WaitUpdate(AIStateUpdateData updateData)
        {
            curActionTimer -= Time.deltaTime;
            if (curActionTimer <= 0f)
            {
                if (RequestNewWanderDestination())
                {
                    wanderAction = WanderAction.Wander;
                    updateData.navigator.SetTarget(updateData.aiGameObjectFacade.data.aiAgentBottom, wanderNavigationTarget);
                }
                else
                {
                    curActionTimer = Random.Range(maxWaitTime, minWaitTime);
                }
            }
        }

        private void WanderUpdate(AIStateUpdateData updateData)
        {
            if (/*strafeHitboxes.frontCollision ||*/ HasReachedDestination(updateData))
            {
                curActionTimer = Random.Range(this.maxWaitTime, this.minWaitTime);
                updateData.navigator.CancelCurrentNavigation();
                wanderAction = WanderAction.Wait;
            }
        }

        private bool RequestNewWanderDestination()
        {
            randomWanderDestination = origin.position;
            randomWanderDestination.x += Random.Range(1f, -1f) * wanderRadius;
            randomWanderDestination.z += Random.Range(1f, -1f) * wanderRadius;

            if (NavMesh.SamplePosition(randomWanderDestination, out hit, NavigatorSettings.onMeshThreshold, NavMesh.AllAreas))
            {
                randomWanderDestination = hit.position;
                wanderNavigationTarget.position = randomWanderDestination;
                return true;
            }
            return false;
        }

        public bool IsWandering()
        {
            return wanderAction == WanderAction.Wander;
        }

        public Transform GetDestinationTransform()
        {
            return wanderNavigationTarget;
        }

        private bool HasReachedDestination(AIStateUpdateData updateData)
        {
            return updateData.navigator.isActivelyGeneratingPath == false;
        }
    }
}
