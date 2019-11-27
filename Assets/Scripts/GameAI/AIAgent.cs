namespace GameAI
{
    using StateHandlers;
    using ComponentInterface;
    using Navigation;

    public class AIAgent
    {
        /// <summary>
        /// Reference to the gameobject holding the enemy class script and component data, which be updated through this non-monobehavior class.
        /// </summary>
        public AIAgentComponentInterface aiAgentComponentInterface;
        private AIStateHandler stateHandler;
        private Navigator navigator;

        public AIAgent(AIAgentComponentInterface newAIAgentComponentInterface)
        {
            aiAgentComponentInterface = newAIAgentComponentInterface;
            aiAgentComponentInterface.Init();
            stateHandler = aiAgentComponentInterface.GetStateHandler();
            navigator = aiAgentComponentInterface.GetNavigator();
            stateHandler.Init(new AIStateUpdateData(aiAgentComponentInterface, TestPlayer.instance, navigator));
            if (aiAgentComponentInterface.aggroZone != null)
            {
                aiAgentComponentInterface.aggroZone.AssignFunctionToTriggerStayDelegate(stateHandler.AggroZoneActivation);
            }
        }

        public void AgentFrameUpdate()
        {
            stateHandler.Update(new AIStateUpdateData(aiAgentComponentInterface, TestPlayer.instance, navigator));
            if (navigator != null)
            {
                navigator.Update();
            }
        }

        public void AgentBeatUpdate()
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
