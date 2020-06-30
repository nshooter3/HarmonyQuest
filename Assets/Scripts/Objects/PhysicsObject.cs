namespace Objects
{
    using UnityEngine;

    public abstract class PhysicsObject : MonoBehaviour
    {
        public abstract void ObjectFixedUpdate();

        public abstract void ObjectLateFixedUpdate();
    }
}
