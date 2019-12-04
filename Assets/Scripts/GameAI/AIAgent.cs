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
            stateHandler = aiGameObject.GetStateHandler();
            navigator = aiGameObject.GetNavigator();
            updateData = new AIStateUpdateData(aiGameObject, TestPlayer.instance, navigator);
            stateHandler.Init(updateData);
            if (aiGameObject.AggroZone != null)
            {
                aiGameObject.AggroZone.AssignFunctionToTriggerStayDelegate(stateHandler.AggroZoneActivation);
            }
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
