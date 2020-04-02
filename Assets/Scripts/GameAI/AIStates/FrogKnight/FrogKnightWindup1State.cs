namespace GameAI.AIStates.FrogKnight
{
    using GameAI.StateHandlers;
    using UnityEngine;

    public class FrogKnightWindup1State : AIState
    {
        public override void Init(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.DebugChangeColor(Color.yellow);
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
            updateData.stateHandler.RequestStateTransition(new FrogKnightWindup2State { }, updateData);
        }

        public override void CheckForStateChange(AIStateUpdateData updateData)
        {
            if (updateData.aiGameObjectFacade.IsDead() == true)
            {
                updateData.stateHandler.RequestStateTransition(new FrogKnightDeadState { }, updateData);
            }
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            aborted = true;
            readyForStateTransition = true;
        }
    }
}
