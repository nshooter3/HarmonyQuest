namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;

    public class CameraFollowPointOfInterest : CameraBehavior
    {
        [SerializeField]
        public TestPlayer player;
        public Transform targetPoint;

        void Awake()
        {
            type = CameraBehaviors.FollowPointOfInterest;
        }

        void Update()
        {
            if (targetPoint != null)
            {
                direction = (targetPoint.position - player.transform.position) * 0.4f;
                direction.y *= direction.y;
            }
            else
            {
                direction = Vector3.zero;
            }
        }
    }
}
