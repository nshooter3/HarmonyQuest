namespace Melody
{
    using GamePhysics;
    using HarmonyQuest;
    using HarmonyQuest.Input;
    using Melody.States;
    using UnityEngine;

    public class MelodyController : MonoBehaviour
    {
        MelodyStateMachine StateMachine;

        public IPlayerInputManager input { get; private set; }

        public Animator animator { get; private set; }
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

        [SerializeField]
        public MelodyConfig config;

        [HideInInspector]
        public Vector3 move;

        //Utility classes
        public MelodyPhysics melodyPhysics;
        public MelodyCollision melodyCollision;
        public MelodyHealth melodyHealth;
        public MelodyHitboxes melodyHitboxes;
        public MelodyAnimator melodyAnimator;

        //Drag References
        public Renderer melodyRenderer;
        public Renderer scarfRenderer;

        // Start is called before the first frame update
        void Start()
        {
            rigidBody = gameObject.GetComponent<Rigidbody>();
            animator = gameObject.GetComponent<Animator>();
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
        }

        // Update is called once per frame
        void Update()
        {
            move.Set(input.GetHorizontalMovement(), 0, input.GetVerticalMovement());
            move = Vector3.ClampMagnitude(move, 1f);
            melodyHitboxes.UpdateHitboxes();
            melodyHealth.OnUpdate(Time.deltaTime);
            StateMachine.OnUpdate(Time.deltaTime);
        }

        void FixedUpdate()
        {
            StateMachine.OnFixedUpdate();
            melodyCollision.OnFixedUpdate();
        }
    }
}
