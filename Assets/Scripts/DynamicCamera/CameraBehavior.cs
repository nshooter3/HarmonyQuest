namespace HarmonyQuest.DynamicCamera
{
    using Melody;
    using UnityEngine;

    public abstract class CameraBehavior
    {
        private IMelodyInfo player;
        
        protected Transform cameraTransform;
        protected Vector3 targetAngles;

        protected float bias = 1f;
        protected bool active = false;
        protected Vector3 direction;

        public virtual void Init(Transform cameraTransform, IMelodyInfo player)
        {
            this.cameraTransform = cameraTransform;
            this.player = player;
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
            return player.GetTransform().position;
        }

        protected Vector3 PlayerVelocity()
        {
            return player.GetVelocity();
        }

        protected Vector3 TargetLocation()
        {
            return player.GetLockonTarget().aiGameObject.transform.position;
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
