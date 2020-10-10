namespace UI
{
    using HarmonyQuest;
    using UnityEngine;

    public class UITracker : MonoBehaviour
    {
        public Transform target;
        float yOffset;

        Camera cam;

        public void OnAwake()
        {
            cam = ServiceLocator.instance.GetCamera();
        }

        public void SetTarget(Transform target, float yOffset = 0.0f)
        {
            this.target = target;
            this.yOffset = yOffset;
        }

        public void ClearTarget()
        {
            target = null;
            yOffset = 0.0f;
        }

        // Update is called once per frame
        public void OnUpdate()
        {
            TrackTarget();
        }

        private void TrackTarget()
        {
            if (target != null)
            {
                Vector3 newPos = cam.WorldToScreenPoint(target.position);
                newPos.y += yOffset;
                transform.position = newPos;
            }
        }
    }
}
