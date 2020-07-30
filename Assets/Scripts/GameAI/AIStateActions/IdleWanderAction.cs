namespace GameAI.AIStateActions
{
    using GameAI.AIGameObjects;
    using GameAI.Navigation;
    using GameAI.StateHandlers;
    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// Class that handles the behavior of an enemy wandering around within a range and waiting when in an idle state.
    /// </summary>
    public class IdleWanderAction
    {
        //Determines what kind of wander action this enemy will perform.
        public enum WanderAction { Wait, Wander };
        public WanderAction wanderAction = WanderAction.Wait;

        float curActionTimer = 0f;
        float maxWaitTime, minWaitTime;

        NavMeshHit hit;

        Transform agentTransform;
        Transform origin;

        Transform wanderNavigationTarget;

        float wanderRadius;

        Vector3 randomWanderDestination;

        float minWanderDistance = 3f;

        bool hitDetected;
        RaycastHit boxcastHit;
        float boxcastRange = 0.7f;
        LayerMask boxcastLayerMask;
        BoxCollider wanderBoxcastReference;

        public IdleWanderAction(AIStateUpdateData updateData, float maxWaitTime = 4.0f, float minWaitTime = 1.0f, float wanderRadius = 10f)
        {
            this.maxWaitTime = maxWaitTime;
            this.minWaitTime = minWaitTime;
            curActionTimer = Random.Range(this.maxWaitTime, this.minWaitTime);
            this.wanderRadius = wanderRadius;
            origin = updateData.aiGameObjectFacade.data.origin;
            agentTransform = updateData.aiGameObjectFacade.transform;
            wanderNavigationTarget = updateData.aiGameObjectFacade.data.wanderNavigationTarget;
            wanderNavigationTarget.parent = null;
            wanderBoxcastReference = updateData.aiGameObjectFacade.data.wanderBoxcastReference;
            boxcastLayerMask = updateData.aiGameObjectFacade.data.wanderBoxcastLayerMask;
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
                if (RequestNewWanderDestination(updateData))
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
            if (CheckForCollision(randomWanderDestination - updateData.aiGameObjectFacade.transform.position) || HasReachedDestination(updateData))
            {
                curActionTimer = Random.Range(this.maxWaitTime, this.minWaitTime);
                updateData.navigator.CancelCurrentNavigation();
                wanderAction = WanderAction.Wait;
                hitDetected = false;
            }
        }

        private bool RequestNewWanderDestination(AIStateUpdateData updateData)
        {
            //Ensure that our new destination is far enough away. This avoids wandering trivial distances.
            bool isNewDestinationFarEnough = false;
            while (isNewDestinationFarEnough == false)
            {
                randomWanderDestination = origin.position;
                randomWanderDestination.x += Random.Range(1f, -1f) * wanderRadius;
                randomWanderDestination.z += Random.Range(1f, -1f) * wanderRadius;
                isNewDestinationFarEnough = Vector3.Distance(updateData.aiGameObjectFacade.data.aiAgentBottom.position, randomWanderDestination) >= minWanderDistance;
            }

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

        private bool CheckForCollision(Vector3 direction)
        {
            hitDetected = Physics.BoxCast(wanderBoxcastReference.transform.position, wanderBoxcastReference.size / 2f, direction, out boxcastHit, Quaternion.LookRotation(direction), boxcastRange, boxcastLayerMask);
            return hitDetected;
        }
    }
}
