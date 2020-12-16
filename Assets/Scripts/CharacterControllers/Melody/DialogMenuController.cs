namespace Melody
{
    using GameManager;
    using HarmonyQuest;
    using HarmonyQuest.Input;

    public class DialogMenuController : ManageableObject
    {
        public IPlayerInputManager input { get; private set; }

        public override void OnStart()
        {
            input = ServiceLocator.instance.GetInputManager();
        }

        public override void OnUpdate()
        {
            CheckInputs();
        }

        void CheckInputs()
        {
            if (input.PauseButtonDown())
            {
                PauseManager.ToggleDialog(false);
                PlayerControllerStateManager.instance.SetState(PlayerControllerStateManager.ControllerState.Melody);
            }
        }
    }
}
