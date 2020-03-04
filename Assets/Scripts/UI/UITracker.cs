namespace UI
{
    using UnityEngine;

    public class UITracker : MonoBehaviour
    {
        Transform target;
        float yOffset;

        Camera cam;

        public void InitTrackingVars(Transform target, Camera cam, float yOffset = 65.0f)
        {
            this.target = target;
            this.cam = cam;
            this.yOffset = yOffset;
        }

        // Update is called once per frame
        void Update()
        {
            TrackTarget();
        }

        private void TrackTarget()
        {
            Vector3 newPos = cam.WorldToScreenPoint(target.position);
            newPos.y += yOffset;
            transform.position = newPos;
        }
    }
}
