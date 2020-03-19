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

        public static bool IsNavMeshBelowTransform(Transform navigationTransform, out Vector3 raycastHitPosition)
        {
            //Start our raycast a little bit above the navigationTransform, so it doesn't fail if it's already touching the navmesh.
            float yOffset = 0.1f;
            Vector3 offsetPosition = new Vector3(navigationTransform.position.x, navigationTransform.position.y + yOffset, navigationTransform.position.z);

            if (Physics.Raycast(offsetPosition, Vector3.down, out RaycastHit raycastHit, Mathf.Infinity, traversableGroundLayerMask))
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

        public static bool IsNavMeshBelowPosition(Vector3 navigationPosition, out Vector3 raycastHitPosition)
        {
            //Start our raycast a little bit above the navigationPosition, so it doesn't fail if it's already touching the navmesh.
            float yOffset = 0.1f;
            Vector3 offsetPosition = new Vector3(navigationPosition.x, navigationPosition.y + yOffset, navigationPosition.z);

            if (Physics.Raycast(offsetPosition, Vector3.down, out RaycastHit raycastHit, Mathf.Infinity, traversableGroundLayerMask))
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
            Vector3 raycastHit;
            if (IsNavMeshBelowPosition(source, out raycastHit))
            {
                startPos = raycastHit;
            }
            if (IsNavMeshBelowPosition(target, out raycastHit))
            {
                targetPos = raycastHit;
            }
            NavMeshHit navmeshRaycastHit;
            return NavMesh.Raycast(startPos, targetPos, out navmeshRaycastHit, areaMask);
        }
    }
}
