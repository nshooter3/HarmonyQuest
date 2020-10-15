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

    public class GameManager : MonoBehaviour
    {
        //References to the prefabs we're gonna load into these objects later.
        public GameObject MelodyController;
        public GameObject AIAgentManager;
        public GameObject PhysicsObjectManager;
        public GameObject TempCamera;
        public GameObject DefaultCanvas;
        public GameObject FmodHandler;
        public GameObject PlayerControllerStateManager;
        public GameObject ServiceLocator;

        //Manager classes that don't have monobehaviors
        public PhysicsManager physicsManager;

        //The master list of all the objects that need to be updated, in order.
        private ObjectManager objectManager = new ObjectManager();

        Transform melodySpawnPoint;

        private void FindMelodySpawnPoint()
        {
            melodySpawnPoint = FindObjectOfType<MelodySpawnPoint>().transform;
        }

        private void PopulateUpdateQueue()
        {
            //Initialize all our one-of manageable objects that need to be in every scene.
            ServiceLocator = Instantiate(ServiceLocator);

            FindMelodySpawnPoint();
            MelodyController = Instantiate(MelodyController, melodySpawnPoint.position, melodySpawnPoint.rotation);

            AIAgentManager = Instantiate(AIAgentManager);
            PhysicsObjectManager = Instantiate(PhysicsObjectManager);
            TempCamera = Instantiate(TempCamera);
            DefaultCanvas = Instantiate(DefaultCanvas);
            FmodHandler = Instantiate(FmodHandler);
            PlayerControllerStateManager = Instantiate(PlayerControllerStateManager);
            ServiceLocator = Instantiate(ServiceLocator);

            //ServiceLocator. Used to get references to other objects in the scene.
            objectManager.AddManageableObject(ServiceLocator.GetComponent<ServiceLocator>());

            //UI
            objectManager.AddManageableObject(DefaultCanvas.GetComponent<UIManager>());

            //Gameplay
            objectManager.AddManageableObject(PlayerControllerStateManager.GetComponent<PlayerControllerStateManager>());
            objectManager.AddManageableObject(AIAgentManager.GetComponent<AIAgentManager>());
            objectManager.AddManageableObject(PhysicsObjectManager.GetComponent<PhysicsObjectManager>());
            //TODO: Use actual camera controller later.
            objectManager.AddManageableObject(TempCamera.GetComponent<TestCamera>());

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
        }

        public void InitManagerClasses()
        {
            physicsManager = new PhysicsManager();
        }

        void Awake()
        {
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
