namespace GameManager
{
    using Effects.Particles;
    using GameAI;
    using GamePhysics;
    using HarmonyQuest.Audio;
    using HarmonyQuest.DynamicCamera;
    using HarmonyQuest.Input.Implementation;
    using Melody;
    using Objects;
    using System.Collections.Generic;
    using UI;
    using UnityEngine;

    public class GameManager : MonoBehaviour
    {
        ObjectManager gameplayObjectManager = new ObjectManager();
        ObjectManager artObjectManager = new ObjectManager();
        ObjectManager uiObjectManager = new ObjectManager();
        ObjectManager audioObjectManager = new ObjectManager();

        //The master list of all the objects that need to be updated, in order.
        List<ManageableObject> updateQueue = new List<ManageableObject>();

        void PopulateUpdateQueue()
        {
            gameplayObjectManager.FindManageableObjectsInScene<RewiredPlayerInputManager>();
            gameplayObjectManager.FindManageableObjectsInScene<MelodyController>();
            gameplayObjectManager.FindManageableObjectsInScene<AIAgentManager>();
            gameplayObjectManager.FindManageableObjectsInScene<PhysicsObjectManager>();
            gameplayObjectManager.FindManageableObjectsInScene<CameraController>();
            gameplayObjectManager.FindManageableObjectsInScene<MelodyGroundedChecker>();
            gameplayObjectManager.FindManageableObjectsInScene<CollisionWrapper>();
            gameplayObjectManager.FindManageableObjectsInScene<DamageHitbox>();
            gameplayObjectManager.FindManageableObjectsInScene<PuzzleZoneTrigger>();
            gameplayObjectManager.FindManageableObjectsInScene<GrapplePoint>();
            gameplayObjectManager.FindManageableObjectsInScene<PushableBoxTrigger>();
            gameplayObjectManager.FindManageableObjectsInScene<CameraPointOfInterest>();

            artObjectManager.FindManageableObjectsInScene<VerletConstraint>();
            artObjectManager.FindManageableObjectsInScene<SetPlayerVelocity>();
            artObjectManager.FindManageableObjectsInScene<ClothPoints>();
            artObjectManager.FindManageableObjectsInScene<BillboardSprite>();
            artObjectManager.FindManageableObjectsInScene<TestScarf>();
            artObjectManager.FindManageableObjectsInScene<DynamicParticleSystem>();

            uiObjectManager.FindManageableObjectsInScene<UITracker>();
            uiObjectManager.FindManageableObjectsInScene<UIMeter>();
            uiObjectManager.FindManageableObjectsInScene<AgentHealthBarsPool>();

            audioObjectManager.FindManageableObjectsInScene<FmodFacade>();
            audioObjectManager.FindManageableObjectsInScene<FmodMusicHandler>();
            audioObjectManager.FindManageableObjectsInScene<FmodOnBeatAccuracyChecker>();
            audioObjectManager.FindManageableObjectsInScene<FmodEventHandler>();
            audioObjectManager.FindManageableObjectsInScene<FmodChordInterpreter>();

            //Add Object Managers to the updateQueue after the null check since they can be incorrectly filtered out for some reason.
            //They perform their own null check anyway.
            updateQueue.Add(gameplayObjectManager);
            updateQueue.Add(artObjectManager);
            updateQueue.Add(uiObjectManager);
            updateQueue.Add(audioObjectManager);
        }

        void Awake()
        {
            PopulateUpdateQueue();
            updateQueue.ForEach(p => p.OnAwake());
        }

        void Start()
        {
            updateQueue.ForEach(p => p.OnStart());
        }
        
        void Update()
        {
            updateQueue.ForEach(p => p.OnUpdate());
        }

        void LateUpdate()
        {
            updateQueue.ForEach(p => p.OnLateUpdate());
        }

        void FixedUpdate()
        {
            updateQueue.ForEach(p => p.OnFixedUpdate());
        }

        void Abort()
        {
            updateQueue.ForEach(p => p.OnAbort());
        }
    }
}
