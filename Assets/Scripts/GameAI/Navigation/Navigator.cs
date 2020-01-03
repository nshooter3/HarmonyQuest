namespace GameAI.Navigation
{
    using UnityEngine;

    public abstract class Navigator
    {
        [HideInInspector]
        public bool isActivelyGeneratingPath = false;

        public Transform navigationTarget;

        public abstract void SetTarget(Transform navigationAgent, Transform navigationTarget);
        public abstract void CancelCurrentNavigation();
        public abstract void CheckIfPathNeedsToBeRegenerated();
        public abstract void CheckIfWaypointIsObstructed();
        public abstract Vector3 GetNextWaypoint();
        public abstract void OnUpdate();
    }
}
