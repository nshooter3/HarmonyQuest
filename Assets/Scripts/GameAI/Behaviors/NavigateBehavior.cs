namespace GameAI.Behaviors
{
    using GameAI.StateHandlers;
    using UnityEngine;

    public class NavigateBehavior : AIBehavior
    {
        public override string GetName()
        {
            return "navigate";
        }

        public override void Start(AIStateUpdateData updateData)
        {
            //Debug.Log("ENTER NAVIGATE STATE");
            updateData.agent.navigator.SetTarget(updateData.agent.aiAgentBottom, updateData.player.transform);
            updateData.agent.targetInLineOfSight = true;
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
