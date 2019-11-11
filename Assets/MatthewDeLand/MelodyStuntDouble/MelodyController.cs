using UnityEngine;

public class MelodyController : MonoBehaviour
{
    readonly CharacterController mCharacterController;
    MelodyStateMachine StateMachine;

    Vector3 move;

    public float MaxSpeed = 5;

    public IPlayerInputManager input { get; private set; }
    public Animator animator { get; private set; }
    public Rigidbody rigidBody { get; private set; }
    public Vector3 Move { get => move; set => move = value; }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = gameObject.GetComponent(typeof(Rigidbody)) as Rigidbody;
        animator = gameObject.GetComponent(typeof(Animator)) as Animator;

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
