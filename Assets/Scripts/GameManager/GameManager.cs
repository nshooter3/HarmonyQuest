namespace GameManager
{
    using DitzelGames.FastIK;
    using Effects.Particles;
    using GameAI;
    using GamePhysics;
    using HarmonyQuest.DynamicCamera;
    using HarmonyQuest.Input.Implementation;
    using Melody;
    using Objects;
    using System.Collections.Generic;
    using System.Linq;
    using UI;
    using UnityEngine;

    public class GameManager : MonoBehaviour
    {
        ObjectManager gameplayObjectManager = new ObjectManager();

        //UI
        UITracker uiTracker;
        UIMeter uiMeter;
        AgentHealthBarsPool agentHealthBarsPool;

        //Art
        FastIKLook fastIKLook;
        FastIKFabric fastIKFabric;
        VerletConstraint verletConstraint;
        SetPlayerVelocity setPlayerVelocity;
        ClothPoints clothPoints;
        BillboardSprite billboardSprite;
        TestScarf testScarf;
        DynamicParticleSystem dynamicParticleSystem;

        //The master list of all the objects that need to be updated, in order.
        List<ManageableObject> updateQueue = new List<ManageableObject>();

        void PopulateUpdateQueue()
        {
            PopulateObjectManagers();

            //Gameplay
            updateQueue.Add(FindObjectOfType<RewiredPlayerInputManager>());
            updateQueue.Add(FindObjectOfType<MelodyController>());
            updateQueue.Add(FindObjectOfType<AIAgentManager>());
            updateQueue.Add(FindObjectOfType<PhysicsObjectManager>());
            updateQueue.Add(FindObjectOfType<CameraController>());
            updateQueue.Add(FindObjectOfType<MelodyGroundedChecker>());
            updateQueue.Add(gameplayObjectManager);

            //Remove null entries as a failsafe.
            updateQueue = updateQueue.Where(item => item != null).ToList();
        }

        void PopulateObjectManagers()
        {
            gameplayObjectManager.FindManageableObjectsInScene<CollisionWrapper>();
            gameplayObjectManager.FindManageableObjectsInScene<DamageHitbox>();
            gameplayObjectManager.FindManageableObjectsInScene<PuzzleZoneTrigger>();
            gameplayObjectManager.FindManageableObjectsInScene<GrapplePoint>();
            gameplayObjectManager.FindManageableObjectsInScene<PushableBoxTrigger>();
            gameplayObjectManager.FindManageableObjectsInScene<CameraPointOfInterest>();

            Debug.Log(gameplayObjectManager);
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
