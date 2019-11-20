namespace GameAI.Navigation
{
    using System.Collections.Generic;
    using UnityEngine.AI;
    using UnityEngine;

    public class GroundedNavmeshNavigator : Navigator
    {
        private Transform navigationAgent;
        private Vector3 lastKnownTargetPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        private NavMeshPath path;
        private Queue<Vector3> waypoints;
        private Vector3 nextWaypoint;

        public override void SetTarget(Transform navigationAgent, Transform navigationTarget)
        {
            this.navigationAgent = navigationAgent;
            this.navigationTarget = navigationTarget;
            path = null;
            waypoints = null;
            lastKnownTargetPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            isActivelyGeneratingPath = true;
            GeneratePathToTarget();
        }

        public override void CancelCurrentNavigation()
        {
            navigationAgent = null;
            navigationTarget = null;
            path = null;
            waypoints = null;
            lastKnownTargetPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            isActivelyGeneratingPath = false;
        }

        public override void CheckIfPathNeedsToBeRegenerated()
        {
            if (isActivelyGeneratingPath == true && navigationTarget != null)
            {
                if (Vector3.Distance(navigationTarget.transform.position, lastKnownTargetPos) > NavigatorSettings.pathRefreshDistanceThreshold)
                {
                    GeneratePathToTarget();
                }
            }
        }

        public override Vector3 GetNextWaypoint()
        {
            return nextWaypoint;
        }

        public override void Update()
        {
            if (isActivelyGeneratingPath == true && navigationTarget != null)
            {
                UpdateDestination();
            }
        }

        private void UpdateDestination()
        {
            if (Vector3.Distance(navigationAgent.position, nextWaypoint) <= NavigatorSettings.waypointReachedDistanceThreshold)
            {
                if (waypoints != null && waypoints.Count > 0)
                {
                    nextWaypoint = waypoints.Dequeue();
                }
            }
        }

        private void GeneratePathToTarget()
        {
            if (navigationAgent == null || navigationTarget == null)
            {
                CancelCurrentNavigation();
                return;
            }

            bool pathFound = false;
            //For both our agent and our target, raycast down to the navmesh before generating a path. This allows navigations to work even when they aren't both grounded.
            if (NavMeshUtil.IsNavMeshBelowTransform(navigationAgent, out Vector3 agentRaycastDownPosition))
            {
                if (NavMeshUtil.IsNavMeshBelowTransform(navigationTarget, out Vector3 targetRaycastDownPosition))
                {
                    path = NavMeshUtil.GeneratePath(agentRaycastDownPosition, targetRaycastDownPosition);

                    if (IsPathToTargetValid())
                    {

                        waypoints = new Queue<Vector3>(path.corners);
                        nextWaypoint = waypoints.Dequeue();
                        lastKnownTargetPos = navigationTarget.transform.position;
                        pathFound = true;
                    }
                }
            }
            if(pathFound == false)
            {
                //If we cannot reach the specified target using the navmesh, cancel navigation.
                CancelCurrentNavigation();
            }
        }

        private bool IsPathToTargetValid()
        {
            if (path == null || path.status == NavMeshPathStatus.PathInvalid)
            {
                return false;
            }
            return true;
        }
    }
}
