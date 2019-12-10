namespace GameAI
{
    using StateHandlers;
    using AIGameObjects;
    using Navigation;

    public class AIAgent
    {
        /// <summary>
        /// Reference to the gameobject holding the enemy class script and component data, which be updated through this non-monobehavior class.
        /// </summary>
        public AIGameObject aiGameObject;
        private AIStateHandler stateHandler;
        private Navigator navigator;

        private AIStateUpdateData updateData;

        public AIAgent(AIGameObject newAIGameObject)
        {
            aiGameObject = newAIGameObject;
            aiGameObject.Init();
            stateHandler = new AIStateHandler();
            navigator = aiGameObject.GetNavigator();
            updateData = new AIStateUpdateData(aiGameObject, stateHandler, navigator, TestPlayer.instance);
            stateHandler.Init(updateData, aiGameObject.GetInitState());
        }

        public void OnUpdate()
        {
            stateHandler.OnUpdate(updateData);
            if (navigator != null)
            {
                navigator.OnUpdate();
            }
        }

        public void OnFixedUpdate()
        {
            stateHandler.OnFixedUpdate(updateData);
        }

        public void OnBeatUpdate()
        {
            stateHandler.OnBeatUpdate(updateData);
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
