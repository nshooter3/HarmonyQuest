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

        private float maxCooldown = 0.5f;
        private float curCooldown = 0f;

        public void Update()
        {
            if (curCooldown > 0f)
            {
                curCooldown -= Time.deltaTime;
            }
        }

        public void StartCooldownTimer()
        {
            curCooldown = maxCooldown;
        }

        public bool IsCooldownActive()
        {
            return curCooldown > 0f;
        }
    }
}
