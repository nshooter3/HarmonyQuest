namespace GameAI.AIGameObjects
{
    using GameAI.Navigation;
    using GameAI.Behaviors;
    using GameAI.Behaviors.FrogKnight;

    public class FrogKnight : Enemy
    {
        public override void Init()
        {
            base.Init();
        }

        public override AIBehavior GetInitState()
        {
            return new FrogKnightIdleBehavior();
        }

        public override Navigator GetNavigator()
        {
            return new GroundedNavmeshNavigator();
        }
    }
}
