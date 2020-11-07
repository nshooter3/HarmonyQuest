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
            cameraTransform.eulerAngles = CalculateRotation();
            cameraTransform.position = CalculateDirection();
            Camera.main.fieldOfView = CalculateFOV();
        }

        public override void Update()
        {
            bias = followBias;
            targetAngles = CalculateRotation();
            direction = CalculateDirection();
            Camera.main.fieldOfView = CalculateFOV();
        }

        private Vector3 CalculateDirection()
        {
            Vector3 position = (cameraTransform.position - PlayerLocation()).normalized * distance;
            position.x = 0;
            position.y = exponentialHeight;
            position.z -= cameraOffset;
            position += PlayerLocation() + PlayerVelocity() / Mathf.Lerp(fastVelocityScale, slowVelocityScale, cameraTransform.position.y / maxHeight);
            return position;
        }

        private Vector3 CalculateRotation()
        {
            // Camera Height calculated on an exponential curve
            exponentialHeight = Mathf.Pow(exponentBase, distance * exponentFactor) + minHeight;
            return Vector3.Lerp(lowAngle, highAngle, exponentialHeight / maxHeight);
        }

        private float CalculateFOV()
        {
            return Mathf.Lerp(wideFOV, narrowFOV, exponentialHeight / maxHeight);
        }
    }
}
