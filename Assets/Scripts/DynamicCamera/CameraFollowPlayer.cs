﻿namespace HarmonyQuest.DynamicCamera
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
        protected float followBias;

        private float exponentialHeight;
        private float exponentFactor = 0.5f;
        private float exponentBase = 1.5f;
        private float cameraOffset = 3.2f;

        void Awake()
        {
            base.Init();
        }

        void Update()
        {
            bias = followBias;
            // Camera Height calculated on an exponential curve
            exponentialHeight = Mathf.Pow(exponentBase, distance * exponentFactor) + minHeight;
            direction = (transform.position - PlayerLocation()).normalized * distance;
            direction.x = 0;
            direction.y = exponentialHeight;
            direction.z -= cameraOffset;
            targetAngles = Vector3.Lerp(lowAngle, highAngle, exponentialHeight / maxHeight);
            direction += PlayerLocation() + PlayerVelocity() / Mathf.Lerp(5f, 1.2f, transform.position.y / maxHeight);
            Camera.main.fieldOfView = Mathf.Lerp(73f, 65f, exponentialHeight / maxHeight);
        }
    }
}
