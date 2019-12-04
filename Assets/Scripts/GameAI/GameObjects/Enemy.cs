namespace GameAI.AIGameObjects
{
    using GameAI.Navigation;
    using GameAI.StateHandlers;

    public abstract class Enemy : AIGameObject
    {
        public override void Init()
        {
            base.Init();
        }

        /// <summary>
        /// Implement this in the child class to specify what kind of state machine this agent will use.
        /// </summary>
        /// <returns> A new instance of this agent's state machine </returns>
        public abstract override AIStateHandler GetStateHandler();

        /// <summary>
        /// Implement this in the child class to specify what kind of navigator this agent will use.
        /// </summary>
        /// <returns> A new instance of this agent's navigator </returns>
        public abstract override Navigator GetNavigator();
    }
}
