namespace GameAI.Agents
{
    using GameAI.Navigation;
    using GameAI.AIStates;
    using GameAI.AIStates.FrogKnight;

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
