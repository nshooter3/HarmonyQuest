namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;

    public class CameraBehaviorFollowPlayer : CameraBehavior
    {
        [Range(2.5f, 10)]
        private float distance = 10f;
        private float minHeight = 0.5f;
        private float maxHeight = 10f;
        private Vector3 lowAngle  = new Vector3(10, 0, 0);
        private Vector3 highAngle = new Vector3(45, 0, 0);
        protected float followBias = 2f;

        private float exponentialHeight;
        private float exponentFactor = 0.5f;
        private float exponentBase = 1.5f;
        private float cameraOffset = 3.2f;
        private float fastVelocityScale = 5f;
        private float slowVelocityScale = 1.2f;
        private float wideFOV = 73f;
        private float narrowFOV = 65f;

        public void ResetToPlayer()
        {
            exponentialHeight = Mathf.Pow(exponentBase, distance * exponentFactor) + minHeight;
            Vector3 position = PlayerLocation();
            position.x = 0;
            position.y = exponentialHeight;
            position.z -= cameraOffset + distance;
            cameraTransform.position = position;
        }

        public override void Update()
        {
            bias = followBias;
            // Camera Height calculated on an exponential curve
            exponentialHeight = Mathf.Pow(exponentBase, distance * exponentFactor) + minHeight;
            direction = (cameraTransform.position - PlayerLocation()).normalized * distance;
            direction.x = 0;
            direction.y = exponentialHeight;
            direction.z -= cameraOffset;
            targetAngles = Vector3.Lerp(lowAngle, highAngle, exponentialHeight / maxHeight);
            direction += PlayerLocation() + PlayerVelocity() / Mathf.Lerp(fastVelocityScale, slowVelocityScale, cameraTransform.position.y / maxHeight);
            Camera.main.fieldOfView = Mathf.Lerp(wideFOV, narrowFOV, exponentialHeight / maxHeight);
        }
    }
}
