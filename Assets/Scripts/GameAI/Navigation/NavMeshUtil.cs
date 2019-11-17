namespace GameAI.Navigation
{
    using UnityEngine.AI;
    using UnityEngine;

    public static class NavMeshUtil
    {
        private static LayerMask traversableGroundLayerMask = LayerMask.GetMask("TraversableGround");

        public static NavMeshPath GeneratePath(Transform source, Transform target, int areaMask = NavMesh.AllAreas)
        {
            return GeneratePath(source.position, target.position, NavMesh.AllAreas);
        }

        public static NavMeshPath GeneratePath(Vector3 source, Transform target, int areaMask = NavMesh.AllAreas)
        {
            return GeneratePath(source, target.position, NavMesh.AllAreas);
        }

        public static NavMeshPath GeneratePath(Transform source, Vector3 target, int areaMask = NavMesh.AllAreas)
        {
            return GeneratePath(source.position, target, NavMesh.AllAreas);
        }

        public static NavMeshPath GeneratePath(Vector3 source, Vector3 target, int areaMask = NavMesh.AllAreas)
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(source, target, areaMask, path);
            return path;
        }

        public static float GetPathLength(NavMeshPath path)
        {
            if (path.status == NavMeshPathStatus.PathInvalid)
            {
                return -1.0f;
            }

            float length = 0.0f;
            for (int i = 1; i < path.corners.Length; ++i)
            {
                length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }

            return length;
        }

        public static bool IsNavMeshBelowAgent(Transform navigationAgent, out Vector3 raycastHitPosition)
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(navigationAgent.position, Vector3.down, out raycastHit, Mathf.Infinity, traversableGroundLayerMask))
            {
                raycastHitPosition = raycastHit.point;
                return true;
            }
            else
            {
                raycastHitPosition = Vector3.zero;
                return false;
            }
        }

        public static bool IsTargetObstructed(Transform source, Transform target, int areaMask = NavMesh.AllAreas)
        {
            Vector3 startPos = source.position;
            Vector3 raycastHit;
            if (IsNavMeshBelowAgent(source, out raycastHit))
            {
                startPos = raycastHit;
            }
            NavMeshHit navmeshRaycastHit;
            return NavMesh.Raycast(startPos, target.position, out navmeshRaycastHit, areaMask);
        }
    }
}
