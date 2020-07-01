namespace GameAI.AIStates
{
    using GameAI.StateHandlers;

    public class TemplateState : AIState
    {
        public override void Init(AIStateUpdateData updateData)
        {

        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {

        }

        public override void OnFixedUpdate(AIStateUpdateData updateData)
        {
            //updateData.aiGameObjectFacade.ApplyVelocity();
            //updateData.aiGameObjectFacade.ApplyGravity();
        }

        public override void OnBeatUpdate(AIStateUpdateData updateData)
        {

        }

        public override void CheckForStateChange(AIStateUpdateData updateData)
        {

        }

        public override void Abort(AIStateUpdateData updateData)
        {
            aborted = true;
            readyForStateTransition = true;
        }
    }
}