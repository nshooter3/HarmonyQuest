namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;

    public abstract class CameraBehavior : MonoBehaviour
    {
        protected float bias = 1f;
        protected bool active = false;
        protected Vector3 direction;
        public CameraBehaviors type { get; protected set; }

        public void Move()
        {
            if (!active)
                return;
            if (!Vector3.Equals(direction, Vector3.zero))
            {
                transform.position = Vector3.Lerp(transform.position, direction, bias * Time.deltaTime);
            }
        }

        public void ToggleActive()
        {
            active = !active;
        }
    }
}
