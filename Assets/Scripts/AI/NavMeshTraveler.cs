namespace AI
{
    using System.Collections.Generic;
    using UnityEngine.AI;
    using UnityEngine;

    public class NavMeshTraveler : MonoBehaviour
    {
        [HideInInspector]
        public bool isTraversalActive = false;
        
        /// <summary>
        /// How frequently we check to see if our path to our target should change.
        /// </summary>
        [SerializeField]
        private float pathRefreshRate = 0.5f;

        /// <summary>
        /// How far our target must move from its last known position to warrant generating a new path.
        /// </summary>
        [SerializeField]
        private float pathRefreshDistanceThreshold = 2.0f;

        /// <summary>
        /// How close we need to be to a waypoint for it to be considered reached.
        /// </summary>
        [SerializeField]
        private float waypointReachedDistanceThreshold = 2.0f;

        private Transform source;
        private Transform target;
        private Vector3 lastKnownTargetPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        private NavMeshPath path;
        private Queue<Vector3> waypoints;
        private Vector3 nextWaypoint;
        private float pathRefreshTimer = 0;

        public void SetTarget(Transform source, Transform target)
        {
            this.source = source;
            this.target = target;
            isTraversalActive = true;
            GeneratePathToTarget();
        }

        public Vector3 GetNextWaypoint()
        {
            return nextWaypoint;
        }

        protected void Update()
        {
            if (isTraversalActive == true && target != null)
            {
                CheckIfPathNeedsToBeRegenerated();
                UpdateDestination();
            }
        }

        void CheckIfPathNeedsToBeRegenerated()
        {
            pathRefreshTimer += Time.deltaTime;
            if (pathRefreshTimer > pathRefreshRate)
            {
                pathRefreshTimer = 0;
                if (Vector3.Distance(target.transform.position, lastKnownTargetPos) > pathRefreshDistanceThreshold)
                {
                    GeneratePathToTarget();
                }
            }
        }

        void UpdateDestination()
        {
            if (Vector3.Distance(source.position, nextWaypoint) <= waypointReachedDistanceThreshold)
            {
                if (waypoints != null && waypoints.Count > 0)
                {
                    nextWaypoint = waypoints.Dequeue();
                    print("GENERATE NEW WAYPOINT: " + nextWaypoint);
                }
            }
        }

        public void GeneratePathToTarget()
        {
            print("GeneratePathToTarget");
            path = NavMeshUtil.GeneratePath(source, target);
            waypoints = new Queue<Vector3>(path.corners);
            nextWaypoint = waypoints.Dequeue();
            lastKnownTargetPos = target.transform.position;
        }
    }
}
