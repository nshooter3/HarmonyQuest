namespace GameAI
{
    using System.Collections.Generic;
    using UnityEngine.AI;
    using UnityEngine;

    public class AgentNavigator : MonoBehaviour
    {
        [HideInInspector]
        public bool isActivelyGeneratingPath = false;
        
        /// <summary>
        /// How frequently we check to see if our path to our target should change.
        /// </summary>
        [SerializeField]
        private float pathRefreshRate = 0.5f;
        private float pathRefreshTimer = 0.0f;

        /// <summary>
        /// How far our target must move from its last known position to warrant generating a new path.
        /// </summary>
        [SerializeField]
        private float pathRefreshDistanceThreshold = 2.0f;

        /// <summary>
        /// How close we need to be to a waypoint for it to be considered reached.
        /// </summary>
        [SerializeField]
        protected float waypointReachedDistanceThreshold = 2.0f;

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
            if (pathRefreshTimer > pathRefreshRate)
            {
                pathRefreshTimer = 0;
                if (Vector3.Distance(navigationTarget.transform.position, lastKnownTargetPos) > pathRefreshDistanceThreshold)
                {
                    GeneratePathToTarget();
                }
            }
        }

        private void UpdateDestination()
        {
            if (Vector3.Distance(navigationAgent.position, nextWaypoint) <= waypointReachedDistanceThreshold)
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
