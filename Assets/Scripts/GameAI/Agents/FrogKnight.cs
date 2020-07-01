namespace GameAI.Agents
{
    using GameAI.Navigation;
    using GameAI.AIStates;
    using GameAI.AIStates.FrogKnight;
    using UnityEngine;

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

        public override AIAnimator GetAnimator()
        {
            return aiAnimator;
        }
    }
}
