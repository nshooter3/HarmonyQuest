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
        }

        public void OnUpdate(AIStateUpdateData updateData)
        {
            switch (wanderAction) {
                case WanderAction.Wait:
                    WaitUpdate();
                    break;
                case WanderAction.Wander:
                    WanderUpdate();
                    break;
            }
        }

        private void WaitUpdate()
        {
            curActionTimer -= Time.deltaTime;
            if (curActionTimer <= 0f)
            {
                if (RequestNewWanderDestination())
                {
                    wanderAction = WanderAction.Wander;
                }
                else
                {
                    curActionTimer = Random.Range(maxWaitTime, minWaitTime);
                }
            }
        }

        private void WanderUpdate()
        {
            if (/*strafeHitboxes.frontCollision ||*/ HasReachedDestination())
            {
                curActionTimer = Random.Range(this.maxWaitTime, this.minWaitTime);
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
                //If our path is blocked, don't wander. Return false.
                return !NavMesh.Raycast(agentTransform.position, randomWanderDestination, out hit, NavMesh.AllAreas);
            }
            return false;
        }

        public bool IsWandering()
        {
            return wanderAction == WanderAction.Wander;
        }

        public Vector3 GetDestination()
        {
            return randomWanderDestination;
        }

        private bool HasReachedDestination()
        {
            return Vector3.Distance(agentTransform.position, randomWanderDestination) <= NavigatorSettings.waypointReachedDistanceThreshold;
        }
    }
}
