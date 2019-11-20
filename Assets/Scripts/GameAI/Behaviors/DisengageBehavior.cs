namespace GameAI.Behaviors
{
    using GameAI.StateHandlers;
    using UnityEngine;

    public class DisengageBehavior : AIBehavior
    {
        public override string GetName()
        {
            return "disengage";
        }

        public override void Start(AIStateUpdateData updateData)
        {
            //Debug.Log("ENTER DISENGAGE STATE");
            updateData.agent.navigator.SetTarget(updateData.agent.aiAgentBottom, updateData.agent.origin);
            updateData.agent.targetInLineOfSight = false;
        }

        public override void Update(AIStateUpdateData updateData)
        {

        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.agent.navigator.CancelCurrentNavigation();
            aborted = true;
            readyForStateTransition = true;
        }
    }
}
