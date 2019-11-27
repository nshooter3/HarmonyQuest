namespace GameAI.Enemies
{
    using GameAI.ComponentInterface;
    using GameAI.Navigation;
    using GameAI.StateHandlers;

    public class FrogKnight : AIAgentComponentInterface
    {
        public override void Init()
        {
            base.Init();
        }

        public override AIStateHandler GetStateHandler()
        {
            return new FrogKnightStateHandler();
        }

        public override Navigator GetNavigator()
        {
            return new GroundedNavmeshNavigator();
        }
    }
}
