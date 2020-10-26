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
        public GameObject TempCamera;
        public GameObject DefaultCanvas;
        public GameObject TransitionManager;
        public GameObject FmodHandler;
        public GameObject PlayerControllerStateManager;
        public GameObject ServiceLocator;

        //Manager classes that don't have monobehaviors
        public PhysicsManager physicsManager;

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
                Debug.Log("LoadDebugFileIfNoFileSelected");
                SaveDataManager.Load(debugSaveFile, out loadError);
            }
        }

        private void PopulateUpdateQueue()
        {
            //Initialize all our one-of manageable objects that need to be in every scene.
            ServiceLocator = Instantiate(ServiceLocator);

            FindMelodySpawnPoint();
            MelodyController = Instantiate(MelodyController, selectedSpawnPoint.transform.position, selectedSpawnPoint.transform.rotation);

            AIAgentManager = Instantiate(AIAgentManager);
            PhysicsObjectManager = Instantiate(PhysicsObjectManager);
            TempCamera = Instantiate(TempCamera);
            DefaultCanvas = Instantiate(DefaultCanvas);
            TransitionManager = Instantiate(TransitionManager);
            FmodHandler = Instantiate(FmodHandler);
            PlayerControllerStateManager = Instantiate(PlayerControllerStateManager);
            ServiceLocator = Instantiate(ServiceLocator);

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
            //TODO: Use actual camera controller later.
            objectManager.AddManageableObject(TempCamera.GetComponent<TestCamera>());

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
            objectManager.FindManageableObjectsInScene<FmodMusicInstructions>();
            objectManager.AddManageableObject(FmodHandler.GetComponent<FmodOnBeatAccuracyChecker>());
            objectManager.AddManageableObject(FmodHandler.GetComponent<FmodChordInterpreter>());
            objectManager.FindManageableObjectsInScene<FmodEventHandler>();
        }

        public void InitManagerClasses()
        {
            physicsManager = new PhysicsManager();
        }

        void Awake()
        {
            LoadDebugFileIfNoFileSelected();
            InitManagerClasses();
            PopulateUpdateQueue();
            objectManager.OnAwake();
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
