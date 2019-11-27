namespace GameAI
{
    using UnityEngine;
    using StateHandlers;
    using ComponentInterface;
    using Navigation;

    public class AIAgent : MonoBehaviour
    {
        public AIAgentComponentInterface aiAgentComponentInterface;
        private AIStateHandler stateHandler;
        private Navigator navigator;

        public virtual void Init()
        {
            if (aiAgentComponentInterface == null && (aiAgentComponentInterface = GetComponentInChildren<AIAgentComponentInterface>()) == null)
            {
                Debug.LogError("AIAgent Init WARNING: Agent is missing a AgentComponentInterface component.");
            }
            aiAgentComponentInterface.Init();
            stateHandler = aiAgentComponentInterface.GetStateHandler();
            navigator = aiAgentComponentInterface.GetNavigator();
            stateHandler.Init(new AIStateUpdateData(aiAgentComponentInterface, TestPlayer.instance, navigator));
            if (aiAgentComponentInterface.aggroZone != null)
            {
                aiAgentComponentInterface.aggroZone.AssignFunctionToTriggerStayDelegate(stateHandler.AggroZoneActivation);
            }
        }

        public virtual void AgentFrameUpdate()
        {
            stateHandler.Update(new AIStateUpdateData(aiAgentComponentInterface, TestPlayer.instance, navigator));
            if (navigator != null)
            {
                navigator.Update();
            }
        }

        public virtual void AgentBeatUpdate()
        {

        }

        public void NavigatorPathRefreshCheck()
        {
            if (navigator != null)
            {
                navigator.CheckIfPathNeedsToBeRegenerated();
            }
        }
    }
}
