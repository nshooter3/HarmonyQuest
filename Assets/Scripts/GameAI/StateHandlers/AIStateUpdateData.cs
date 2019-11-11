namespace GameAI.StateHandlers
{
    public class AIStateUpdateData
    {
        public AIAgent agent;
        public TestPlayer player;
        public AgentNavigator navigator;

        public AIStateUpdateData(AIAgent agent, TestPlayer player, AgentNavigator navigator)
        {
            this.agent = agent;
            this.player = player;
            this.navigator = navigator;
        }
    }
}
