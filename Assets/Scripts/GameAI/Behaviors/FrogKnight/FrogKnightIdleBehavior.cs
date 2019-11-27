namespace GameAI.Behaviors.FrogKnight
{
    using GameAI.StateHandlers;
    using UnityEngine;

    public class FrogKnightIdleBehavior : AIBehavior
    {
        public override string GetName()
        {
            return "idle";
        }

        public override void Start(AIStateUpdateData updateData)
        {
            updateData.agentComponentInterface.targetInLineOfSight = false;
            updateData.agentComponentInterface.SetRigidbodyConstraints(RigidbodyConstraints.FreezeAll);
        }

        public override void Update(AIStateUpdateData updateData)
        {

        }

        public override void Abort(AIStateUpdateData updateData)
        {
            aborted = true;
            readyForStateTransition = true;
        }
    }
}
