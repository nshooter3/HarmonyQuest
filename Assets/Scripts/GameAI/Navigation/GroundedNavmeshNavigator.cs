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

        public override bool SetTarget(Transform navigationAgent, Transform navigationTarget)
        {
            this.navigationAgent = navigationAgent;
            this.navigationTarget = navigationTarget;
            path = null;
            waypoints = null;
            lastKnownTargetPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            isActivelyGeneratingPath = true;
            return GeneratePathToTarget();
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

        public override void RegeneratePathIfTargetHasMoved()
        {
            if (isActivelyGeneratingPath == true && navigationTarget != null)
            {
                if (Vector3.Distance(navigationTarget.transform.position, lastKnownTargetPos) > NavigatorSettings.pathRefreshDistanceThreshold)
                {
                    //If the target has moved far enough away from their previous position, generate a new path.
                    GeneratePathToTarget();
                }
            }
        }

        public override void RegeneratePathIfWaypointIsObstructed()
        {
            if (isActivelyGeneratingPath == true && navigationTarget != null)
            {
                if (NavMeshUtil.IsTargetObstructed(navigationAgent.transform, nextWaypoint))
                {
                    //If this agent no longer has a direct path to its current waypoint, generate a new path.
                    GeneratePathToTarget();
                }
            }
        }

        public override Vector3 GetNextWaypoint()
        {
            return nextWaypoint;
        }

        public override void OnUpdate()
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
                else
                {
                    CancelCurrentNavigation();
                }
            }
        }

        private bool GeneratePathToTarget()
        {
            if (navigationAgent == null || navigationTarget == null)
            {
                CancelCurrentNavigation();
                return false;
            }

            bool pathFound = false;
            //For both our agent and our target, raycast down to the navmesh before generating a path. This allows navigations to work even when they aren't both grounded.
            if (NavMeshUtil.IsAgentOnNavMesh(navigationAgent.position, out NavMeshHit agentNavMeshHit))
            {
                if (NavMeshUtil.IsAgentOnNavMesh(navigationTarget.position, out NavMeshHit targetNavMeshHit))
                {
                    path = NavMeshUtil.GeneratePath(agentNavMeshHit.position, targetNavMeshHit.position);

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
            return pathFound;
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
