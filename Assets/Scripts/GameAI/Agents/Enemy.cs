namespace GameAI.Agents
{
    using GameAI.AIStates;
    using GameAI.Navigation;
    using GameAI.AIGameObjects;

    public abstract class Enemy : AIGameObjectFacade
    {
        public override void Init()
        {
            base.Init();
        }

        /// <summary>
        /// Implement this in the child class to specify which state the enemy will start in.
        /// </summary>
        /// <returns> A new instance of this agent's initial state </returns>
        public override abstract AIState GetInitState();

        /// <summary>
        /// Implement this in the child class to specify what kind of navigator this agent will use.
        /// </summary>
        /// <returns> A new instance of this agent's navigator </returns>
        public abstract override Navigator GetNavigator();
    }
}
