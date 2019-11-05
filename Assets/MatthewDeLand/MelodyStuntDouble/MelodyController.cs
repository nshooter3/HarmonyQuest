using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelodyController : MonoBehaviour
{
    CharacterController mCharacterController;
    Animator mAnimator;
    IPlayerInputManager mInput;
    Rigidbody mRigidBody;

    bool lockedInAnimation = false;

    MelodyStateMachine StateMachine;

    Vector3 move;

    public float MaxSpeed = 2;

    public IPlayerInputManager MInput { get => mInput; }
    public Animator MAnimator { get => mAnimator; }
    public CharacterController MCharacterController { get => mCharacterController; }
    public Rigidbody MRigidBody { get => mRigidBody; }
    public Vector3 Move { get => move; set => move = value; }

    // Start is called before the first frame update
    void Start()
    {
        //mCharacterController = gameObject.GetComponent(typeof(CharacterController)) as CharacterController;
        mRigidBody = gameObject.GetComponent(typeof(Rigidbody)) as Rigidbody;
        mAnimator = gameObject.GetComponent(typeof(Animator)) as Animator;

        mInput = ServiceLocator.instance.GetInputManager();

        StateMachine = new MelodyStateMachine(this);

        move = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();

        move.Set(mInput.GetHorizontalMovement(), 0, mInput.GetVerticalMovement());
        StateMachine.OnUpdate(Time.deltaTime);
    }


    void CheckInput()
    {
        /*
        if (mInput.AttackButtonDown())
        {
            mAnimator.SetBool("Attack", true);
        }

        if (mInput.ParryButtonDown())
        {
            mAnimator.SetBool("Counter", true);
        }
        */
    }
}
