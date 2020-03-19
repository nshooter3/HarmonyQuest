namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;

    public abstract class CameraBehavior
    {
        private TestPlayer player;
        private CharacterController characterController;
        
        protected Transform cameraTransform;
        protected Vector3 targetAngles;

        protected float bias = 1f;
        protected bool active = false;
        protected Vector3 direction;

        public virtual void Init(Transform cameraTransform, TestPlayer player)
        {
            this.cameraTransform = cameraTransform;
            this.player = player;
            characterController = player.GetComponent<CharacterController>();
        }

        public virtual void Update()
        {

        }

        public void Move()
        {
            if (!active)
                return;
            if (!Vector3.Equals(direction, Vector3.zero))
            {
                cameraTransform.position = Vector3.Lerp(cameraTransform.position, direction, bias * Time.deltaTime);
            }
            cameraTransform.eulerAngles = Vector3.Lerp(cameraTransform.eulerAngles, targetAngles, bias * Time.deltaTime);
        }

        public void ToggleActive(bool value)
        {
            active = value;
        }
        
        protected Vector3 PlayerLocation()
        {
            return player.transform.position;
        }

        protected Vector3 PlayerVelocity()
        {
            return characterController.velocity;
        }

        protected Vector3 TargetLocation()
        {
            return player.lockOnTarget.transform.position;
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
