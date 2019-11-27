namespace GameAI.StateHandlers
{
    using ComponentInterface;
    using Navigation;
    public class AIStateUpdateData
    {
        public AIAgentComponentInterface agentComponentInterface;
        public TestPlayer player;
        public Navigator navigator;

        public AIStateUpdateData(AIAgentComponentInterface agentComponentInterface, TestPlayer player, Navigator navigator)
        {
            this.agentComponentInterface = agentComponentInterface;
            this.player = player;
            this.navigator = navigator;
        }
    }
}
