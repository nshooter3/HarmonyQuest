namespace GameAI.AIGameObjects
{
    using GameAI.Navigation;
    using GameAI.StateHandlers;

    public class FrogKnight : Enemy
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
