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

        public AIAgent(AIGameObject newAIGameObject)
        {
            aiGameObject = newAIGameObject;
            aiGameObject.Init();
            stateHandler = aiGameObject.GetStateHandler();
            navigator = aiGameObject.GetNavigator();
            stateHandler.Init(new AIStateUpdateData(aiGameObject, TestPlayer.instance, navigator));
            if (aiGameObject.AggroZone != null)
            {
                aiGameObject.AggroZone.AssignFunctionToTriggerStayDelegate(stateHandler.AggroZoneActivation);
            }
        }

        public void OnUpdate()
        {
            stateHandler.OnUpdate(new AIStateUpdateData(aiGameObject, TestPlayer.instance, navigator));
            if (navigator != null)
            {
                navigator.OnUpdate();
            }
        }

        public void OnFixedUpdate()
        {
            stateHandler.OnFixedUpdate(new AIStateUpdateData(aiGameObject, TestPlayer.instance, navigator));
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
