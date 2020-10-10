namespace HarmonyQuest
{
    using GameAI;
    using Input;
    using Melody;
    using UnityEngine;
    using UI;
    using GameManager;

    ///<summary>
    ///The ServiceLocator is a Singleton that points scripts to whatever is managing each of the game's major subsystems.
    ///This script should be attached to a GameObject in a scene. If a viable manager for each subsystem is attached to the same
    ///GameObject, that manager will be used. Otherwise, the ServiceManager will do nothing, unless a manager for that subsystem is requested,
    ///in which case, the ServiceLocator will try to start one up. Useful for testing!
    ///</summary>
    public class ServiceLocator : ManageableObject
    {
        public static ServiceLocator instance;

        //List of managers the ServiceLocator can provide
        IPlayerInputManager InputManager;
        MelodyController melodyController;
        IMelodyInfo melodyInfo;
        AIAgentManager aiAgentManager;
        UIManager uiManager;
        //TODO: Make this use Mitch's camera.
        Camera cam;
        //End List of managers

        public override void OnAwake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            melodyController = FindObjectOfType<MelodyController>();
            melodyInfo = melodyController.GetComponent(typeof(IMelodyInfo)) as IMelodyInfo;
            aiAgentManager = FindObjectOfType<AIAgentManager>();
            uiManager = FindObjectOfType<UIManager>();
            cam = FindObjectOfType<Camera>();
            //Check to see to see what Managers have been attached to this GameObject.
            InputManager = GetComponent(typeof(IPlayerInputManager)) as IPlayerInputManager;
            InputManager.OnAwake();
        }

        public IPlayerInputManager GetInputManager()
        {
            return InputManager;
        }

        public MelodyController GetMelodyController()
        {
            return melodyController;
        }

        public IMelodyInfo GetMelodyInfo()
        {
            return melodyInfo;
        }

        public AIAgentManager GetAIAgentManager()
        {
            return aiAgentManager;
        }

        public UIManager GetUIManager()
        {
            return uiManager;
        }

        public Camera GetCamera()
        {
            return cam;
        }
    }
}
