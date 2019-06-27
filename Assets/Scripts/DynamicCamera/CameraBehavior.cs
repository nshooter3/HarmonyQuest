namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;

    public abstract class CameraBehavior : MonoBehaviour
    {
        protected float bias;
        protected bool active = false;
        protected Vector3 direction;
        public CameraBehaviors type { get; protected set; }

        public void Move()
        {
            if (!active)
                return;
            transform.position += direction;
        }

        public void ToggleActive()
        {
            active = !active;
        }
    }
}
