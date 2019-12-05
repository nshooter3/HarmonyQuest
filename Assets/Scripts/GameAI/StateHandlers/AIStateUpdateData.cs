namespace GameAI.StateHandlers
{
    using AIGameObjects;
    using Navigation;
    public class AIStateUpdateData
    {
        public AIGameObject aiGameObject;
        public AIStateHandler stateHandler;
        public Navigator navigator;
        public TestPlayer player;

        public AIStateUpdateData(AIGameObject aiGameObject, AIStateHandler stateHandler, Navigator navigator, TestPlayer player)
        {
            this.aiGameObject = aiGameObject;
            this.stateHandler = stateHandler;
            this.navigator = navigator;
            this.player = player;
        }
    }
}
