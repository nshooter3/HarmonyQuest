namespace GameAI
{
    using UnityEngine.AI;
    using UnityEngine;

    public static class NavMeshUtil
    {
        public static NavMeshPath GeneratePath(Transform source, Transform target, int areaMask = NavMesh.AllAreas)
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(source.position, target.transform.position, areaMask, path);
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

        public static bool IsTargetObstructed(Transform source, Transform target, int areaMask = NavMesh.AllAreas)
        {
            NavMeshHit hit;
            return NavMesh.Raycast(source.position, target.position, out hit, areaMask);
        }
    }
}
