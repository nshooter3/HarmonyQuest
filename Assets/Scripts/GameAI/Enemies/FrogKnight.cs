namespace GameAI
{
    using GameAI.StateHandlers;

    public class FrogKnight : Enemy
    {
        public override void Init()
        {
            states = new BasicEnemyStateHandler();
            navigator = new AgentNavigator();
            base.Init();
        }

        public override void AgentFrameUpdate()
        {
            base.AgentFrameUpdate();
        }

        public override void AgentBeatUpdate()
        {
            base.AgentBeatUpdate();
        }
    }
}
