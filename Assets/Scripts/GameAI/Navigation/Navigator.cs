namespace GameAI.Navigation
{
    using UnityEngine;

    public abstract class Navigator
    {
        [HideInInspector]
        public bool isActivelyGeneratingPath = false;

        public Transform navigationTarget;

        public abstract bool SetTarget(Transform navigationAgent, Transform navigationTarget);
        public abstract void CancelCurrentNavigation();
        public abstract void RegeneratePathIfTargetHasMoved();
        public abstract void RegeneratePathIfWaypointIsObstructed();
        public abstract Vector3 GetNextWaypoint();
        public abstract void OnUpdate();
    }
}
