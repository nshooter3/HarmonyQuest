namespace GameAI.Behaviors
{
    using GameAI.StateHandlers;
    using UnityEngine;

    public class IdleBehavior : AIBehavior
    {
        public override string GetName()
        {
            return "idle";
        }

        public override void Start(AIStateUpdateData updateData)
        {
            //Debug.Log("ENTER IDLE STATE");
            updateData.agent.targetInLineOfSight = false;
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
