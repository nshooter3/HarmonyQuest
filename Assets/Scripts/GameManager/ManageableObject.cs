namespace GameManager
{
    using UnityEngine;

    public abstract class ManageableObject : MonoBehaviour
    {
        //Track whether or not this object has run its start and awake functions. Prevents repeat calls if an object is marked as DontDestroyOnLoad.
        public bool hasInitialized = false;

        public virtual void OnAwake() { }

        public virtual void OnStart() { }

        public virtual void OnUpdate() { }

        public virtual void OnLateUpdate() { }

        public virtual void OnFixedUpdate() { }

        public virtual void OnAbort() { }
    }
}
