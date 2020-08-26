namespace Objects
{
    using UnityEngine;

    public class GrapplePoint : MonoBehaviour
    {
        public Transform visibleDestination;
        public Transform actualDestination;
        public int priority = 1;

        public enum GrapplePointType { LandAbove, LandBelow };
        public GrapplePointType grapplePointType;

        public bool active = true;
    }
}
