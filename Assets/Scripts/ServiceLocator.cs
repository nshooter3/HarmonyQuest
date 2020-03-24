namespace HarmonyQuest
{
    using HarmonyQuest.Input.Implementation;
    using Input;
    using Input.Implementation;
    using Melody;
    using UnityEngine;

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

        //List of managers the ServiceLocator can provide
        IPlayerInputManager InputManager;
        MelodyController melodyController;
        //End List of managers
       
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
        }

        void Start()
        {
            //Check to see to see what Managers have been attached to this GameObject.
            InputManager = GetComponent(typeof(IPlayerInputManager)) as IPlayerInputManager;
        }

        public IPlayerInputManager GetInputManager()
        {
            if(InputManager != null)
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
    }
}
