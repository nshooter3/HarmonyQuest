namespace GameManager
{
    using UnityEngine;

    public abstract class ManageableObject : MonoBehaviour
    {
        public virtual void OnAwake() { }

        public virtual void OnStart() { }

        public virtual void OnUpdate() { }

        public virtual void OnLateUpdate() { }

        public virtual void OnFixedUpdate() { }

        public virtual void OnAbort() { }
    }
}
