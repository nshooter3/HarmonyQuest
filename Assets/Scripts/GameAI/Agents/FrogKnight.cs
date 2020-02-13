namespace GameAI.Agents
{
    using GameAI.AIGameObjects;
    using GameAI.Navigation;
    using GameAI.States;
    using GameAI.States.FrogKnight;

    public class FrogKnight : Enemy
    {
        public override void Init()
        {
            base.Init();
        }

        public override AIState GetInitState()
        {
            return new FrogKnightIdleState();
        }

        public override Navigator GetNavigator()
        {
            return new GroundedNavmeshNavigator();
        }
    }
}
