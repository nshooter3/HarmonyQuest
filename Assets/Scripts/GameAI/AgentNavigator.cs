namespace GameAI
{
    using System.Collections.Generic;
    using UnityEngine.AI;
    using UnityEngine;

    public class AgentNavigator : MonoBehaviour
    {
        [HideInInspector]
        public bool isActivelyGeneratingPath = false;
        
        private float pathRefreshTimer = 0.0f;

        protected Transform navigationAgent;
        protected Transform navigationTarget;
        private Vector3 lastKnownTargetPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        private NavMeshPath path;
        private Queue<Vector3> waypoints;
        private Vector3 nextWaypoint;

        private RaycastHit raycastHit;
        private Vector3 raycastHitPosition;

        public void SetTarget(Transform navigationAgent, Transform navigationTarget)
        {
            this.navigationAgent = navigationAgent;
            this.navigationTarget = navigationTarget;
            path = null;
            waypoints = null;
            lastKnownTargetPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            isActivelyGeneratingPath = true;
            GeneratePathToTarget();
        }

        public void CancelCurrentNavigation()
        {
            navigationAgent = null;
            navigationTarget = null;
            path = null;
            waypoints = null;
            lastKnownTargetPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            isActivelyGeneratingPath = false;
        }

        public Vector3 GetNextWaypoint()
        {
            return nextWaypoint;
        }

        protected void Update()
        {
            if (isActivelyGeneratingPath == true && navigationTarget != null)
            {
                UpdateDestination();
                CheckIfPathNeedsToBeRegenerated();
            }
        }

        private void CheckIfPathNeedsToBeRegenerated()
        {
            pathRefreshTimer += Time.deltaTime;
            if (pathRefreshTimer > NavigatorSettings.pathRefreshRate)
            {
                pathRefreshTimer = 0;
                if (Vector3.Distance(navigationTarget.transform.position, lastKnownTargetPos) > NavigatorSettings.pathRefreshDistanceThreshold)
                {
                    GeneratePathToTarget();
                }
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
            bool pathFound = false;
            if (NavMeshUtil.IsNavMeshBelowAgent(navigationAgent, out raycastHitPosition))
            {
                path = NavMeshUtil.GeneratePath(raycastHitPosition, navigationTarget);

                if (path.status != NavMeshPathStatus.PathInvalid)
                {

                    waypoints = new Queue<Vector3>(path.corners);
                    nextWaypoint = waypoints.Dequeue();
                    lastKnownTargetPos = navigationTarget.transform.position;
                    pathFound = true;
                }
            }
            if(pathFound == false)
            {
                //If we cannot reach the specified target using the navmesh, cancel navigation.
                CancelCurrentNavigation();
            }
        }

        protected bool IsPathToTargetValid()
        {
            if (path == null || path.status == NavMeshPathStatus.PathInvalid)
            {
                return false;
            }
            return true;
        }
    }
}
