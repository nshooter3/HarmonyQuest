namespace HarmonyQuest
{
    using GameAI;
    using HarmonyQuest.Input.Implementation;
    using Input;
    using Melody;
    using UnityEngine;
    using UI;

    ///<summary>
    ///The ServiceLocator is a Singleton that points scripts to whatever is managing each of the game's major subsystems.
    ///This script should be attached to a GameObject in a scene. If a viable manager for each subsystem is attached to the same
    ///GameObject, that manager will be used. Otherwise, the ServiceManager will do nothing, unless a manager for that subsystem is requested,
    ///in which case, the ServiceLocator will try to start one up. Useful for testing!
    ///</summary>
    public class ServiceLocator : MonoBehaviour
    {
        private static ServiceLocator inst;
        public static ServiceLocator instance
        {
            get
            {
                if (!inst)
                {
                    inst = GameObject.FindObjectOfType<ServiceLocator>();
                }
                return inst;
            }
        }

        public GameObject DefaultUIManager;

        //List of managers the ServiceLocator can provide
        IPlayerInputManager InputManager;
        MelodyController melodyController;
        AIAgentManager aiAgentManager;
        UIManager uiManager;
        //End List of managers

        //TODO: Make this use Mitch's camera.
        Camera cam;

        void Awake()
        {
            if (inst == null)
            {
                inst = this;
            }
            else
            {
                Destroy(gameObject);
            }
            melodyController = FindObjectOfType<MelodyController>();
            aiAgentManager = FindObjectOfType<AIAgentManager>();
            uiManager = FindObjectOfType<UIManager>();
            cam = Object.FindObjectOfType<Camera>();
        }

        void Start()
        {
            //Check to see to see what Managers have been attached to this GameObject.
            InputManager = GetComponent(typeof(IPlayerInputManager)) as IPlayerInputManager;
        }

        public IPlayerInputManager GetInputManager()
        {
            if (InputManager != null)
            {
                return InputManager;
            }
            else
            {
                Debug.LogWarning("No Input Manager detected to handle request. Starting one up");
                gameObject.AddComponent(typeof(BasicPlayerInputManager));
                InputManager = GetComponent(typeof(IPlayerInputManager)) as IPlayerInputManager;
                return InputManager;
            }
        }

        public MelodyController GetMelodyController()
        {
            if (melodyController == null)
            {
                //If GetMelodyController gets called before we have a chance to set melodyController, do so here.
                melodyController = FindObjectOfType<MelodyController>();
                if (melodyController == null)
                {
                    //If melodyController is still null, we have a problem.
                    Debug.LogError("No MelodyController detected in scene.");
                    return null;
                }
            }
            return melodyController;
        }

        public AIAgentManager GetAIAgentManager()
        {
            if (aiAgentManager == null)
            {
                //If GetAIAgentManager gets called before we have a chance to set aiAgentManager, do so here.
                aiAgentManager = FindObjectOfType<AIAgentManager>();
                if (aiAgentManager == null)
                {
                    //If aiAgentManager is still null, we have a problem.
                    Debug.LogWarning("No AIAgentManager detected in scene. A Default one will be created.");
                    GameObject aam = new GameObject();
                    aam.AddComponent(typeof(AIAgentManager));
                    aiAgentManager = aam.GetComponent(typeof(AIAgentManager)) as AIAgentManager;
                    return aiAgentManager;
                }
            }
            return aiAgentManager;
        }

        public UIManager GetUIManager()
        {
            if (uiManager == null)
            {
                //If GetUIManager gets called before we have a chance to set uiManager, do so here.
                uiManager = FindObjectOfType<UIManager>();
                if (uiManager == null)
                {
                    //If uiManager is still null, we have a problem.
                    Debug.LogWarning("No UIManager detected in scene. A Default one will be created.");
                    GameObject go = Instantiate(DefaultUIManager);
                    uiManager = go.GetComponent(typeof(UIManager)) as UIManager;
                    return uiManager;
                }
            }
            return uiManager;
        }

        public Camera GetCamera()
        {
            if (cam == null)
            {
                //If GetCamera gets called before we have a chance to set cam, do so here.
                cam = FindObjectOfType<Camera>();
                if (cam == null)
                {
                    //If cam is still null, we have a problem.
                    Debug.LogError("No Camera detected in scene.");
                    return null;
                }
            }
            return cam;
        }
    }
}
