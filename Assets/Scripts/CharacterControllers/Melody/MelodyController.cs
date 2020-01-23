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

        // Start is called before the first frame update
        void Start()
        {
            rigidBody = gameObject.GetComponent(typeof(Rigidbody)) as Rigidbody;
            animator = gameObject.GetComponent(typeof(Animator)) as Animator;
            config = gameObject.GetComponent(typeof(MelodyConfig)) as MelodyConfig;

            input = ServiceLocator.instance.GetInputManager();

            StateMachine = new MelodyStateMachine(this);

            move = new Vector3();
        }

        // Update is called once per frame
        void Update()
        {
            move.Set(input.GetHorizontalMovement(), 0, input.GetVerticalMovement());
            StateMachine.OnUpdate(Time.deltaTime);
        }
    }
}
