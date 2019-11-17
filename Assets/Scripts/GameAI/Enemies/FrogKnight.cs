namespace GameAI.Enemies
{
    using GameAI.StateHandlers;
    using Navigation;

    public class FrogKnight : Enemy
    {
        public override void Init()
        {
            stateHandler = new BasicEnemyStateHandler();
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
