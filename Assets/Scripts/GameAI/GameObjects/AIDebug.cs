namespace GameAI.AIGameObjects
{
    using UnityEngine;

    public class AIDebug
    {
        private AIGameObjectData data;

        public void Init(AIGameObjectData data)
        {
            this.data = data;
        }

        public virtual void DebugChangeColor(Color color)
        {
            data.body.material.color = color;
            data.head.material.color = color;
        }
    }
}

