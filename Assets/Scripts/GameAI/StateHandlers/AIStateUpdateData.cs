namespace GameAI.StateHandlers
{
    using AIGameObjects;
    using Navigation;
    public class AIStateUpdateData
    {
        public AIGameObjectFacade aiGameObjectFacade;
        public AIStateHandler stateHandler;
        public Navigator navigator;
        public TestPlayer player;

        public AIStateUpdateData(AIGameObjectFacade aiGameObject, AIStateHandler stateHandler, Navigator navigator, TestPlayer player)
        {
            this.aiGameObjectFacade = aiGameObject;
            this.stateHandler = stateHandler;
            this.navigator = navigator;
            this.player = player;
        }
    }
}
