namespace GameAI
{
    using StateHandlers;
    using AIGameObjects;
    using Navigation;
    using HarmonyQuest;
    using UnityEngine;

    public class AIAgent
    {
        /// <summary>
        /// Reference to the gameobject holding the enemy class script and component data, which be updated through this non-monobehavior class.
        /// </summary>
        public AIGameObjectFacade aiGameObject;
        private AIStateHandler stateHandler;
        private Navigator navigator;
        private AIAnimator animator;

        private AIStateUpdateData updateData;

        public AIAgent(AIGameObjectFacade newAIGameObject)
        {
            aiGameObject = newAIGameObject;
            aiGameObject.Init();
            stateHandler = new AIStateHandler();
            navigator = aiGameObject.GetNavigator();
            animator  = aiGameObject.GetAnimator();
            updateData = new AIStateUpdateData(aiGameObject, stateHandler, animator, navigator, ServiceLocator.instance.GetMelodyInfo());
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
                navigator.RegeneratePathIfTargetHasMoved();
            }
        }

        public void NavigatorsWaypointIsObstructedCheck()
        {
            if (navigator != null)
            {
                navigator.RegeneratePathIfWaypointIsObstructed();
            }
        }
    }
}
