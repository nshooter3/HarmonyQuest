namespace GameAI.AIStates.FrogKnight
{
    using GameAI.StateHandlers;

    public class FrogKnightAggroState : AIState
    {
        public override void Init(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.aiSound.PlayFmodEvent("enemy_aggro");
            updateData.stateHandler.RequestStateTransition(new FrogKnightEngageState { }, updateData);
        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {

        }

        public override void OnFixedUpdate(AIStateUpdateData updateData)
        {
            
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
