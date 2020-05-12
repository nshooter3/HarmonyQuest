namespace GameAI.StateHandlers
{
    using AIGameObjects;
    using Melody;
    using Navigation;
    public class AIStateUpdateData
    {
        public AIGameObjectFacade aiGameObjectFacade;
        public AIStateHandler stateHandler;
        public Navigator navigator;
        public IMelodyInfo player;
        public AIAnimator animator;

        public AIStateUpdateData(AIGameObjectFacade aiGameObjectFacade, AIStateHandler stateHandler, AIAnimator animator, Navigator navigator, IMelodyInfo player)
        {
            this.aiGameObjectFacade = aiGameObjectFacade;
            this.stateHandler = stateHandler;
            this.navigator = navigator;
            this.player = player;
            this.animator = animator;
        }
    }
}
