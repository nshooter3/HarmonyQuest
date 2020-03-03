namespace UI
{
    using UnityEngine;

    public class UITrackingMeter : UIMeter
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
        new void Update()
        {
            TrackTarget();
            base.Update();
        }

        private void TrackTarget()
        {
            Vector3 newPos = cam.WorldToScreenPoint(target.position);
            newPos.y += yOffset;
            transform.position = newPos;
        }
    }
}
