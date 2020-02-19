namespace GameAI.AIStateActions
{
    using GameAI.StateHandlers;
    using UnityEngine;

    public class AttackAction
    {
        private void AttackWindup(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.DebugChangeColor(Color.yellow);
        }

        private void Attack(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.DebugChangeColor(Color.red);
        }
    }
}
