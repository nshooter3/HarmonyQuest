namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;

    public class CameraFollowPlayer : CameraBehavior
    {
        [SerializeField]
        public TestPlayer player;
        private CharacterController characterController;
        public Vector3 distanceFromPlayer;

        void Awake()
        {
            type = CameraBehaviors.FollowPlayer;
            characterController = player.GetComponent<CharacterController>();
        }

        void Update()
        {
            bias = 2f;
            direction = PlayerLocation() + PlayerVelocity() / 1.2f;
            if (IsLockedOn())
            {
                direction = Vector3.Lerp(TargetLocation(), PlayerLocation(), 0.5f);
                bias = 9f;
            }
            direction +=  distanceFromPlayer;
        }

        private Vector3 PlayerLocation()
        {
            return player.transform.position;
        }

        private Vector3 PlayerVelocity()
        {
            if (!IsFollowingPOI())
                return characterController.velocity;
            else
                return Vector3.zero;
        }

        private Vector3 TargetLocation()
        {
            return player.lockOnTarget.transform.position;
        }

        private bool IsLockedOn()
        {
            return player.IsLockedOn();
        }

        private bool IsFollowingPOI()
        {
            return (CameraController.instance.GetCameraBehavior(CameraBehaviors.FollowPointOfInterest) as CameraFollowPointOfInterest).targetPoint != null;
        }

        Vector3 ScaleVectorComponents(Vector3 v, float xScale, float yScale, float zScale)
        {
            v.x *= xScale;
            v.y *= yScale;
            v.z *= zScale;
            return v;
        }
    }
}
