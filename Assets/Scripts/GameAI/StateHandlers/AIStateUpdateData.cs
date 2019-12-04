namespace GameAI.StateHandlers
{
    using AIGameObjects;
    using Navigation;
    public class AIStateUpdateData
    {
        public AIGameObject aiGameObject;
        public TestPlayer player;
        public Navigator navigator;

        public AIStateUpdateData(AIGameObject aiGameObject, TestPlayer player, Navigator navigator)
        {
            this.aiGameObject = aiGameObject;
            this.player = player;
            this.navigator = navigator;
        }
    }
}
