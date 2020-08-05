namespace Melody
{
    using GameAI;
    using GamePhysics;
    using HarmonyQuest;
    using HarmonyQuest.Input;
    using Melody.States;
    using UnityEngine;

    public class MelodyController : MonoBehaviour, IMelodyInfo
    {
        MelodyStateMachine StateMachine;
        public string currentStateName;

        public IPlayerInputManager input { get; private set; }

        public Animator animator;
        public Animator Animator { get { return animator; } private set { animator = value; } }
        public Rigidbody rigidBody { get; private set; }
        public CollisionWrapper melodyColliderWrapper { get; private set; }
        public CapsuleCollider capsuleCollider { get; private set; }

        /// <summary>
        /// The hurtboxes for Melody.
        /// </summary>
        [Tooltip("The hurtboxes for Melody.")]
        public Collider[] hurtboxes;
        public Collider counterHurtbox;
        public MeshRenderer counterHurtboxMesh;

        /// <summary>
        /// The CounterDamageReceiver for the player. Handles taking counter damage without needing a damage hitbox.
        /// </summary>
        [Tooltip("The CounterDamageReceiver for the player. Handles taking counter damage without needing a damage hitbox.")]
        public CounterDamageReceiver counterDamageReceiver;

        [SerializeField]
        public MelodyConfig config;

        [HideInInspector]
        public Vector3 move;

        [HideInInspector]
        public Vector2 rightStickMove;

        //Utility classes
        public MelodyPhysics melodyPhysics;
        public MelodyCollision melodyCollision;
        public MelodyHealth melodyHealth;
        public MelodyHitboxes melodyHitboxes;
        public MelodyAnimator melodyAnimator;
        public MelodyLockOn melodyLockOn;
        //MelodySound is actually a monobehavior, so it will be assigned via drag reference.
        public MelodySound melodySound;

        //Drag References
        public GameObject melodyRenderer;
        public Renderer scarfRenderer;

        // Start is called before the first frame update
        void Start()
        {
            rigidBody = gameObject.GetComponent<Rigidbody>();
            melodyColliderWrapper = gameObject.GetComponent<CollisionWrapper>();
            capsuleCollider = gameObject.GetComponent<CapsuleCollider>();

            input = ServiceLocator.instance.GetInputManager();

            StateMachine = new MelodyStateMachine(this);

            move = new Vector3();

            melodyPhysics = new MelodyPhysics(this);
            melodyCollision = new MelodyCollision(this);
            melodyHealth = new MelodyHealth(this);
            melodyHitboxes = new MelodyHitboxes(this);
            melodyAnimator = new MelodyAnimator(this);
            melodyLockOn = new MelodyLockOn(this);
            melodySound.Init(this, ServiceLocator.instance.GetAIAgentManager());
        }

        // Update is called once per frame
        void Update()
        {
            CheckInputs();
            melodyHitboxes.UpdateHitboxes();
            melodyHealth.OnUpdate(Time.deltaTime);
            melodyLockOn.OnUpdate(Time.deltaTime);
            StateMachine.OnUpdate(Time.deltaTime);
            melodySound.OnUpdate();
            currentStateName = StateMachine.GetCurrentStateName();
            //Debug.Log("State: " + currentStateName);
        }

        void FixedUpdate()
        {
            StateMachine.OnFixedUpdate();
            melodyCollision.OnFixedUpdate();
            melodySound.OnFixedUpdate();
        }

        void CheckInputs()
        {
            move.Set(input.GetHorizontalMovement(), 0, input.GetVerticalMovement());
            move = Vector3.ClampMagnitude(move, 1f);

            rightStickMove.Set(input.GetHorizontalMovement2(), input.GetVerticalMovement2());

            if (input.LockonButtonDown())
            {
                melodyLockOn.LockonButtonPressed();
            }
            else if (melodyLockOn.HasLockonTarget() == true && rightStickMove.magnitude > 0.5f)
            {
                melodyLockOn.ChangeLockonTargetRightStick(rightStickMove);
            }
            if (rightStickMove.magnitude <= 0.25f)
            {
                melodyLockOn.RightStickResetToNeutral();
            }
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public Vector3 GetTransformForward()
        {
            return transform.forward;
        }

        public MelodyConfig GetConfig()
        {
            return config;
        }

        public AIAgent GetLockonTarget()
        {
            return melodyLockOn.GetLockonTarget();
        }

        public bool HasLockonTarget()
        {
            return melodyLockOn.HasLockonTarget();
        }

		public float GetCurrentHealth()
        {
            return melodyHealth.GetCurrentHealth();
        }

        public float GetMaxHealth()
        {
            return MelodyStats.maxHealth;
        }

        public MelodySound GetMelodySound()
        {
            return melodySound;
        }

		public Vector3 GetVelocity()
        {
            return melodyPhysics.GetVelocity();
        }
    }
}
