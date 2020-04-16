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

        public bool SeekDestinationIfOutOfRange(AIGameObjectFacade aiGameObject, Vector3 target, float range, bool ignoreYValue = true, float speedModifier = 1.0f, bool alwaysFaceTarget = true)
        {
            if (Vector3.Distance(aiGameObject.transform.position, target) > range)
            {
                aiGameObject.SetVelocityTowardsDestination(target, ignoreYValue, speedModifier, alwaysFaceTarget);
                return true;
            }
            return false;
        }

        public void SeekDirection(AIGameObjectFacade aiGameObject, Vector3 direction, bool ignoreYValue = true, float speedModifier = 1.0f, bool alwaysFaceTarget = true)
        {
            aiGameObject.SetVelocity(direction, ignoreYValue, speedModifier, alwaysFaceTarget);
        }
    }
}
