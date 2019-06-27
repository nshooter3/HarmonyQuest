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
            direction = PlayerVelocity() / 1.2f;
            if (IsLockedOn())
            {
                direction = Vector3.Lerp(TargetLocation(), PlayerLocation(), 0.5f);
                bias = 9f;
            }
            // direction += distanceFromPlayer;
            // direction = Vector3.Lerp(PlayerLocation(), direction, 2f * Time.deltaTime);
        }

        private Vector3 PlayerLocation()
        {
            return player.transform.position;
        }

        private Vector3 PlayerVelocity()
        {
            return characterController.velocity;
        }

        private Vector3 TargetLocation()
        {
            return player.lockOnTarget.transform.position;
        }

        private bool IsLockedOn()
        {
            return player.IsLockedOn();
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
