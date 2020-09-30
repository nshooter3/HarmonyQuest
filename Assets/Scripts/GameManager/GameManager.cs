namespace GameManager
{
    using Effects.Particles;
    using GameAI;
    using GamePhysics;
    using HarmonyQuest;
    using HarmonyQuest.Audio;
    using HarmonyQuest.DynamicCamera;
    using HarmonyQuest.Input.Implementation;
    using Manager;
    using Melody;
    using Objects;
    using System.Collections.Generic;
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
        public GameObject ServiceLocator;

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
            ServiceLocator = Instantiate(ServiceLocator);

            //Gameplay
            objectManager.AddManageableObject(MelodyController.GetComponent<MelodyController>());
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

            //UI
            objectManager.FindManageableObjectsInScene<UITracker>();
            objectManager.FindManageableObjectsInScene<UIMeter>();
            objectManager.FindManageableObjectsInScene<AgentHealthBarsPool>();

            //Audio
            objectManager.AddManageableObject(FmodHandler.GetComponent<FmodFacade>());
            objectManager.AddManageableObject(FmodHandler.GetComponent<FmodMusicHandler>());
            objectManager.AddManageableObject(FmodHandler.GetComponent<FmodOnBeatAccuracyChecker>());
            objectManager.AddManageableObject(FmodHandler.GetComponent<FmodChordInterpreter>());
            objectManager.FindManageableObjectsInScene<FmodEventHandler>();

            //Misc
            objectManager.AddManageableObject(ServiceLocator.GetComponent<ServiceLocator>());
        }

        void Awake()
        {
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
