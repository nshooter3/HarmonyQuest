namespace UI
{
    using HarmonyQuest;
    using UnityEngine;

    public class UITracker : MonoBehaviour
    {
        public Transform target;
        public float yOffset;
        public float xOffset;

        private RectTransform rectTransform;

        Camera cam;

        public void OnEnable()
        {
            if(ServiceLocator.instance != null)
            {
                ServiceLocator.instance.GetUIManager().AddUITracker(this);
                cam = ServiceLocator.instance.GetCamera();
            }           
            rectTransform = GetComponent<RectTransform>();
        }

        public void OnDisable()
        {
            ServiceLocator.instance.GetUIManager().RemoveUITracker(this);
        }

        public void OnAwake()
        {
            cam = ServiceLocator.instance.GetCamera();
            rectTransform = GetComponent<RectTransform>();
        }

        public void SetTarget(Transform target, float yOffset = 0.0f, float xOffset = 0.0f)
        {
            this.target = target;
            this.yOffset = yOffset;
            this.xOffset = xOffset;
        }

        public void SetOffsets(float xOffset, float yOffset)
        {
            this.yOffset = xOffset;
            this.xOffset = yOffset;
            TrackTarget();
        }

        public void ClearTarget()
        {
            target = null;
            yOffset = 0.0f;
            xOffset = 0.0f;
        }

        // Update is called once per frame
        public void OnUpdate()
        {
            TrackTarget();
        }

        private void TrackTarget()
        {
            if (target != null && cam != null)
            {
                Vector2 newPos = cam.WorldToScreenPoint(target.position);
                float scaleFactor = ServiceLocator.instance.GetUIManager().canvas.scaleFactor;
                newPos = new Vector2(newPos.x / scaleFactor, newPos.y / scaleFactor);
                newPos.y += yOffset;
                newPos.x += xOffset;

                rectTransform.anchoredPosition = newPos;
            }
        }
    }
}
