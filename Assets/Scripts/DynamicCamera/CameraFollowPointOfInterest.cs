namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;

    public class CameraFollowPointOfInterest : CameraBehavior
    {
        [SerializeField]
        public TestPlayer player;
        public Transform targetPoint;
        public Vector3 distanceFromPlayer;

        void Awake()
        {
            type = CameraBehaviors.FollowPointOfInterest;
            bias = 1.5f;
        }

        void Update()
        {
            if (targetPoint != null)
            {
                direction = Vector3.Lerp(player.transform.position, targetPoint.position, 0.5f) + distanceFromPlayer;
            }
            else
            {
                direction = Vector3.zero;
            }
        }
    }
}
