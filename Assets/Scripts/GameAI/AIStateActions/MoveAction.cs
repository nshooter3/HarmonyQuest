namespace GameAI.AIStateActions
{
    using GameAI.AIGameObjects;
    using UnityEngine;

    public class MoveAction
    {
        public  void SeekDestination(AIGameObjectFacade aiGameObject, Vector3 target, bool ignoreYValue = true, float speedModifier = 1.0f, bool alwaysFaceTarget = true)
        {
            aiGameObject.SetVelocityTowardsDestination(target, ignoreYValue, speedModifier, alwaysFaceTarget);
        }

        public void SeekDirection(AIGameObjectFacade aiGameObject, Vector3 direction, bool ignoreYValue = true, float speedModifier = 1.0f, bool alwaysFaceTarget = true)
        {
            aiGameObject.SetVelocity(direction, ignoreYValue, speedModifier, alwaysFaceTarget);
        }
    }
}
