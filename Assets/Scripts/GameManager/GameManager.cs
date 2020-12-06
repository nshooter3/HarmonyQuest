namespace GameManager
{
    using Effects.Particles;
    using GameAI;
    using GamePhysics;
    using HarmonyQuest;
    using HarmonyQuest.Audio;
    using HarmonyQuest.DynamicCamera;
    using Manager;
    using Melody;
    using Objects;
    using UI;
    using UnityEngine;
    using Saving;
    using System.Collections.Generic;

    public class GameManager : MonoBehaviour
    {
        //References to the prefabs we're gonna load into these objects later.
        public GameObject MelodyController;
        public GameObject AIAgentManager;
        public GameObject PhysicsObjectManager;
        public GameObject CameraController;
        public GameObject DefaultCanvas;
        public GameObject TransitionManager;
        public GameObject FmodHandler;
        public GameObject PlayerControllerStateManager;
        public GameObject ServiceLocator;
        public GameObject DialogManager;

        //Variables to hold scripts found in the scene upon load.
        //This is so that objects that aren't destroyed on load can be reloaded into the update queue when switching scenes.
        private FmodFacade foundFmodHandler;
        private UITransitionManager foundTransitionManager;

        //Manager classes that don't have monobehaviors
        private PhysicsManager physicsManager;
        private SceneLoadObjectDictionary sceneLoadObjectDictionary;

        //The master list of all the objects that need to be updated, in order.
        private ObjectManager objectManager = new ObjectManager();

        List<MelodySpawnPoint> melodySpawnPoints;
        MelodySpawnPoint selectedSpawnPoint;

        [SerializeField]
        private int debugSaveFile = 1;

        private string loadError;

        private void FindMelodySpawnPoint()
        {
            melodySpawnPoints = new List<MelodySpawnPoint>(FindObjectsOfType<MelodySpawnPoint>());
            selectedSpawnPoint = melodySpawnPoints.Find(x => x.id == SaveDataManager.saveData.currentDoor);
            if (selectedSpawnPoint == null)
            {
                selectedSpawnPoint = melodySpawnPoints[0];
                Debug.LogWarning("No doors matched the id " + SaveDataManager.saveData.currentDoor + ", so spawning the player at first available door.");
            }
        }

        //If we haven't entered a scene through the main menu, load a save file just in case.
        private void LoadDebugFileIfNoFileSelected()
        {
            if (SaveDataManager.saveLoaded == false)
            {
                //Debug.Log("LoadDebugFileIfNoFileSelected");
                SaveDataManager.Load(debugSaveFile, out loadError);
            }
        }

        private void PopulateUpdateQueue()
        {
            //Initialize all our one-of manageable objects that need to be in every scene.
            ServiceLocator = Instantiate(ServiceLocator);
            FindMelodySpawnPoint();
            
            MelodyController = Instantiate(MelodyController, selectedSpawnPoint.transform.position, selectedSpawnPoint.transform.rotation);
            foundTransitionManager = FindObjectOfType<UITransitionManager>();
            if (foundTransitionManager != null)
            {
                TransitionManager = foundTransitionManager.gameObject;
            }
            else
            {
                TransitionManager = Instantiate(TransitionManager);
            }
            foundFmodHandler = FindObjectOfType<FmodFacade>();
            if (foundFmodHandler != null)
            {
                FmodHandler = foundFmodHandler.gameObject;
            }
            else
            {
                FmodHandler = Instantiate(FmodHandler);
            }
            AIAgentManager = Instantiate(AIAgentManager);
            PhysicsObjectManager = Instantiate(PhysicsObjectManager);
            CameraController = Instantiate(CameraController);
            DefaultCanvas = Instantiate(DefaultCanvas);
            PlayerControllerStateManager = Instantiate(PlayerControllerStateManager);
            ServiceLocator = Instantiate(ServiceLocator);
            DialogManager = Instantiate(DialogManager);

            //ServiceLocator. Used to get references to other objects in the scene.
            objectManager.AddManageableObject(ServiceLocator.GetComponent<ServiceLocator>());

            //UI
            objectManager.AddManageableObject(DefaultCanvas.GetComponent<UIManager>());
            objectManager.AddManageableObject(TransitionManager.GetComponent<UITransitionManager>());
            objectManager.FindManageableObjectsInScene<UITransition>();

            //Gameplay
            objectManager.AddManageableObject(PlayerControllerStateManager.GetComponent<PlayerControllerStateManager>());
            objectManager.AddManageableObject(AIAgentManager.GetComponent<AIAgentManager>());
            objectManager.AddManageableObject(PhysicsObjectManager.GetComponent<PhysicsObjectManager>());
            objectManager.AddManageableObject(CameraController.GetComponent<CameraController>());

            objectManager.FindManageableObjectsInScene<SceneTransitionTrigger>();
            objectManager.FindManageableObjectsInScene<CollisionWrapper>();
            objectManager.FindManageableObjectsInScene<DamageHitbox>();
            objectManager.FindManageableObjectsInScene<PuzzleZoneTrigger>();
            objectManager.FindManageableObjectsInScene<GrapplePoint>();
            objectManager.FindManageableObjectsInScene<PushableBoxTrigger>();
            objectManager.FindManageableObjectsInScene<CameraPointOfInterest>();
            objectManager.FindManageableObjectsInScene<MelodySpawnPoint>();

            //Art
            objectManager.FindManageableObjectsInScene<VerletConstraint>();
            objectManager.FindManageableObjectsInScene<SetPlayerVelocity>();
            objectManager.FindManageableObjectsInScene<ClothPoints>();
            objectManager.FindManageableObjectsInScene<BillboardSprite>();
            objectManager.FindManageableObjectsInScene<TestScarf>();
            objectManager.FindManageableObjectsInScene<DynamicParticleSystem>();

            //Audio
            objectManager.AddManageableObject(FmodHandler.GetComponent<FmodFacade>());
            objectManager.AddManageableObject(FmodHandler.GetComponent<FmodMusicHandler>());
            objectManager.AddManageableObject(FmodHandler.GetComponent<FmodOnBeatAccuracyChecker>());
            objectManager.AddManageableObject(FmodHandler.GetComponent<FmodChordInterpreter>());
            objectManager.FindManageableObjectsInScene<FmodEventHandler>();

            //Dialog
            objectManager.AddManageableObject(DialogManager.GetComponent<DialogController>()) ;
        }

        public void InitManagerClasses()
        {
            if (SceneLoadObjectDictionary.instance == null)
            {
                sceneLoadObjectDictionary = new SceneLoadObjectDictionary();
            }
            physicsManager = new PhysicsManager();
        }

        void Awake()
        {
            LoadDebugFileIfNoFileSelected();
            InitManagerClasses();
            PopulateUpdateQueue();
            objectManager.OnAwake();
            FmodFacade.instance.LoadSceneMusic();
        }

        void Start()
        {
            objectManager.OnStart();
        }

        void Update()
        {
            objectManager.OnUpdate();
        }

        void LateUpdate()
        {
            objectManager.OnLateUpdate();
        }

        void FixedUpdate()
        {
            objectManager.OnFixedUpdate();
        }

        void Abort()
        {
            objectManager.OnAbort();
        }
    }
}
