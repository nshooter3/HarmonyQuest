namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;

    public class CameraFollowPointOfInterest : CameraBehavior
    {
        public Transform targetPoint;
        public Vector3 distanceFromPlayer;

        void Awake()
        {
            base.Init();
            bias = 1.5f;
        }

        void Update()
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
