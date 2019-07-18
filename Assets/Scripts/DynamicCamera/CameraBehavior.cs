namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;

    public abstract class CameraBehavior : MonoBehaviour
    {
        [SerializeField]
        private TestPlayer player;
        private CharacterController characterController;

        protected float bias = 1f;
        protected bool active = false;
        protected Vector3 direction;

        protected void Init()
        {
            characterController = player.GetComponent<CharacterController>();
        }

        public void Move()
        {
            if (!active)
                return;
            if (!Vector3.Equals(direction, Vector3.zero))
            {
                transform.position = Vector3.Lerp(transform.position, direction, bias * Time.deltaTime);
                // Logarithmic?
                // transform.position = transform.position - (transform.position -  direction) * Mathf.Pow(Time.deltaTime,  bias);
            }
        }

        public void ToggleActive()
        {
            active = !active;
        }

        
        protected Vector3 PlayerLocation()
        {
            return player.transform.position;
        }

        protected Vector3 PlayerVelocity()
        {
            if (!IsFollowingPOI())
                return characterController.velocity;
            else
                return Vector3.zero;
        }

        protected Vector3 TargetLocation()
        {
            return player.lockOnTarget.transform.position;
        }

        protected bool IsLockedOn()
        {
            return player.IsLockedOn();
        }

        protected bool IsFollowingPOI()
        {
            return CameraController.instance.GetCameraBehavior<CameraFollowPointOfInterest>().targetPoint;
        }

        protected Vector3 ScaleVectorComponents(Vector3 v, float xScale, float yScale, float zScale)
        {
            v.x *= xScale;
            v.y *= yScale;
            v.z *= zScale;
            return v;
        }
    }
}
