namespace UI
{
    using HarmonyQuest;
    using UnityEngine;

    public class UITracker : MonoBehaviour
    {
        public Transform target;
        float yOffset;

        private RectTransform rectTransform;

        Camera cam;

        public void OnAwake()
        {
            cam = ServiceLocator.instance.GetCamera();
            rectTransform = GetComponent<RectTransform>();
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
                Vector2 newPos = cam.WorldToScreenPoint(target.position);
                float scaleFactor = ServiceLocator.instance.GetUIManager().canvas.scaleFactor;
                newPos = new Vector2(newPos.x / scaleFactor, newPos.y / scaleFactor);
                newPos.y += yOffset;

                rectTransform.anchoredPosition = newPos;
            }
        }
    }
}
