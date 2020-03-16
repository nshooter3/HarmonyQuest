namespace Melody
{
    using HarmonyQuest;
    using HarmonyQuest.Input;
    using Melody.States;
    using UnityEngine;

    public class MelodyController : MonoBehaviour
    {
        MelodyStateMachine StateMachine;

        Vector3 move;

        public IPlayerInputManager input { get; private set; }
        public Animator animator { get; private set; }
        public Rigidbody rigidBody { get; private set; }
        public Vector3 Move { get => move; set => move = value; }
        public MelodyConfig config { get; private set; }

        //Drag References
        public Renderer melodyRenderer;
        public Renderer scarfRenderer;

        // Start is called before the first frame update
        void Start()
        {
            rigidBody = gameObject.GetComponent<Rigidbody>();
            animator = gameObject.GetComponent<Animator>();
            config = gameObject.GetComponent<MelodyConfig>();

            input = ServiceLocator.instance.GetInputManager();

            StateMachine = new MelodyStateMachine(this);

            move = new Vector3();
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
        }
    }
}
