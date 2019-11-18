namespace GameAI.Behaviors
{
    using GameAI.Navigation;
    using GameAI.StateHandlers;
    using UnityEngine;

    public class EngageBehavior : AIBehavior
    {
        private float checkForTargetObstructionTimer = 0.0f;

        public override string GetName()
        {
            return "engage";
        }

        public override void Start(AIStateUpdateData updateData)
        {
            Debug.Log("ENTER ENGAGE STATE");
            updateData.agent.targetInLineOfSight = true;
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
