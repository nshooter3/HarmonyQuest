namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;

    public class CameraFollowPlayer : CameraBehavior
    {
        [Range(2.5f, 10)]
        public float distance;
        public float minHeight;
        public float maxHeight;
        public Vector3 lowAngle;
        public Vector3 highAngle;
        [SerializeField]
        private Vector3 cameraOffset;
        [SerializeField]
        protected float followBias;

        void Awake()
        {
            base.Init();
        }

        void Update()
        {
            bias = followBias;
            float exponentialHeight = Mathf.Pow(1.5f, distance / 2.0f) + minHeight;
            direction = (transform.position - PlayerLocation()).normalized * distance;
            direction.x = 0;
            direction.y = exponentialHeight;
            direction.z -= 1.6f;
            exponentialHeight = Mathf.Pow(1.5f, distance/1.5f) + minHeight;
            targetAngles = Vector3.Lerp(lowAngle, highAngle, exponentialHeight / maxHeight);
            direction += PlayerLocation() + PlayerVelocity() / Mathf.Lerp(5f, 1.2f, transform.position.y / maxHeight);
            Camera.main.fieldOfView = Mathf.Lerp(73f, 65f, exponentialHeight / maxHeight);
        }
    }
}
