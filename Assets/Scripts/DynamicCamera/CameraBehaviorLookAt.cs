namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;

    public class CameraBehaviorLookAt : CameraBehavior
    {
        public Transform targetPoint;
        private Vector3 distanceFromPlayer = new Vector3(0, 9.5f, -10);

        public override void Init(Transform cameraTransform, TestPlayer player)
        {
            base.Init(cameraTransform, player);
            bias = 1.5f;
            targetAngles = new Vector3(45, 0, 0);
        }

        public override void Update()
        {
            if (targetPoint != null)
            {
                direction = Vector3.Lerp(PlayerLocation(), targetPoint.position, 0.5f) + distanceFromPlayer;
            }
            else
            {
                direction = Vector3.zero;
            }
        }
    }
}
