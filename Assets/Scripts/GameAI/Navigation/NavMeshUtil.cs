namespace GameAI.Navigation
{
    using UnityEngine.AI;
    using UnityEngine;

    public static class NavMeshUtil
    {
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

        public static bool IsAgentOnNavMesh(Vector3 agentPosition, out NavMeshHit hit)
        {
            // Check for nearest point on navmesh to agent, within onMeshThreshold
            if (NavMesh.SamplePosition(agentPosition, out hit, NavigatorSettings.onMeshThreshold, NavMesh.AllAreas))
            {
                return true;
            }

            return false;
        }

        public static bool IsTargetObstructed(Transform source, Transform target, int areaMask = NavMesh.AllAreas)
        {
            return IsTargetObstructed(source.position, target.position, areaMask);
        }

        public static bool IsTargetObstructed(Vector3 source, Transform target, int areaMask = NavMesh.AllAreas)
        {
            return IsTargetObstructed(source, target.position, areaMask);
        }

        public static bool IsTargetObstructed(Transform source, Vector3 target, int areaMask = NavMesh.AllAreas)
        {
            return IsTargetObstructed(source.position, target, areaMask);
        }

        public static bool IsTargetObstructed(Vector3 source, Vector3 target, int areaMask = NavMesh.AllAreas)
        {
            Vector3 startPos = source;
            Vector3 targetPos = target;
            NavMeshHit navMeshHit;
            if (IsAgentOnNavMesh(source, out navMeshHit))
            {
                startPos = navMeshHit.position;
            }
            if (IsAgentOnNavMesh(target, out navMeshHit))
            {
                targetPos = navMeshHit.position;
            }
            NavMeshHit navmeshRaycastHit;
            return NavMesh.Raycast(startPos, targetPos, out navmeshRaycastHit, areaMask);
        }
    }
}
