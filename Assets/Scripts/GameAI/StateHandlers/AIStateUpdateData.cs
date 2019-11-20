namespace GameAI.StateHandlers
{
    using Navigation;
    public class AIStateUpdateData
    {
        public AIAgent agent;
        public TestPlayer player;
        public Navigator navigator;

        public AIStateUpdateData(AIAgent agent, TestPlayer player, Navigator navigator)
        {
            this.agent = agent;
            this.player = player;
            this.navigator = navigator;
        }
    }
}
