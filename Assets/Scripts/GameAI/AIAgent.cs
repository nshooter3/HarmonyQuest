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
            if (aiAgentComponentInterface.AggroZone != null)
            {
                aiAgentComponentInterface.AggroZone.AssignFunctionToTriggerStayDelegate(stateHandler.AggroZoneActivation);
            }
        }

        public void Update()
        {
            stateHandler.Update(new AIStateUpdateData(aiAgentComponentInterface, TestPlayer.instance, navigator));
            if (navigator != null)
            {
                navigator.Update();
            }
        }

        public void FixedUpdate()
        {
            stateHandler.FixedUpdate(new AIStateUpdateData(aiAgentComponentInterface, TestPlayer.instance, navigator));
        }

        public void BeatUpdate()
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
