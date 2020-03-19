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

        [SerializeField]
        public MelodyConfig config;

        public Vector3 move;

        //Utility classes
        public MelodyPhysics melodyPhysics;
        public MelodyCollision melodyCollision;

        //Drag References
        public Renderer melodyRenderer;
        public Renderer scarfRenderer;

        // Start is called before the first frame update
        void Start()
        {
            rigidBody = gameObject.GetComponent<Rigidbody>();
            animator = gameObject.GetComponent<Animator>();
            melodyColliderWrapper = gameObject.GetComponent<CollisionWrapper>();

            input = ServiceLocator.instance.GetInputManager();

            StateMachine = new MelodyStateMachine(this);

            move = new Vector3();

            melodyPhysics = new MelodyPhysics(this);
            melodyCollision = new MelodyCollision(this);
        }

        // Update is called once per frame
        void Update()
        {
            move.Set(input.GetHorizontalMovement(), 0, input.GetVerticalMovement());
            move = Vector3.ClampMagnitude(move, 1f);
            StateMachine.OnUpdate(Time.deltaTime);
        }

        void FixedUpdate()
        {
            StateMachine.OnFixedUpdate();
            melodyCollision.OnFixedUpdate();
        }
    }
}
