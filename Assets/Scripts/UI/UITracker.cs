namespace UI
{
    using HarmonyQuest;
    using UnityEngine;

    public class UITracker : MonoBehaviour
    {
        public Transform target;
        public float yOffset;
        public float xOffset;

        Camera cam;

        public void Start()
        {
            cam = ServiceLocator.instance.GetCamera();
            float[] scaledPixels = ServiceLocator.instance.GetUIManager().GetScaledPixels((int) xOffset, (int)yOffset);
            yOffset = scaledPixels[1];
            xOffset = scaledPixels[0];
        }

        public void SetTarget(Transform target, float yOffset = 0.0f, float xOffset = 0.0f)
        {
            this.target = target;
            //this.yOffset = ServiceLocator.instance.GetUIManager().GetScaledPixels(0, (int)yOffset)[1];
            //this.xOffset = ServiceLocator.instance.GetUIManager().GetScaledPixels(0, (int) xOffset)[0];
        }

        public void SetOffsets(float xOffset, float yOffset)
        {
            float[] scaledPixels = ServiceLocator.instance.GetUIManager().GetScaledPixels((int)xOffset, (int)yOffset);
            this.yOffset = scaledPixels[1];
            this.xOffset = scaledPixels[0];
            TrackTarget();
        }

        public void ClearTarget()
        {
            target = null;
            yOffset = 0.0f;
            xOffset = 0.0f;
        }

        // Update is called once per frame
        void Update()
        {
            TrackTarget();
        }

        private void TrackTarget()
        {
            if (target != null)
            {
                Vector3 newPos = cam.WorldToScreenPoint(target.position);
                newPos.y += yOffset;
                newPos.x += xOffset;
                transform.position = newPos;
            }
        }
    }
}
