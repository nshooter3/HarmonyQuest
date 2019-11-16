namespace GameAI
{
    using GameAI.StateHandlers;
    using UnityEngine;

    public abstract class AIAgent : MonoBehaviour
    {
        protected AIStateHandler stateHandler;
        protected AgentNavigator navigator;

        public abstract void Init();
        public abstract void AgentFrameUpdate();
        public abstract void AgentBeatUpdate();

        public void UpdateState()
        {
            stateHandler.Update(new AIStateUpdateData(this, TestPlayer.instance, navigator));
        }
    }
}
