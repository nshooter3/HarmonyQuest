namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;

    public class CameraFollowPlayer : CameraBehavior
    {
        public Vector3 distanceFromPlayer;
        [SerializeField]
        private float followBias;

        void Awake()
        {
            base.Init();
        }

        void Update()
        {
            bias = followBias;
            direction = PlayerLocation() + PlayerVelocity() / 1.2f;
            if (IsLockedOn())
            {
                direction = Vector3.Lerp(TargetLocation(), PlayerLocation(), 0.5f);
                bias = 9f;
            }
            direction +=  distanceFromPlayer;
        }
    }
}
