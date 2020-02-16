namespace GameAI.AIGameObjects
{
    using UnityEngine;

    public class AICollision
    {
        private AIGameObjectData data;

        public void Init(AIGameObjectData data)
        {
            this.data = data;
        }

        public virtual Collider[] GetHurtboxes()
        {
            return data.hurtboxes;
        }

        public virtual Collider GetCollisionAvoidanceHitbox()
        {
            return data.collisionAvoidanceHitbox;
        }
    }
}
