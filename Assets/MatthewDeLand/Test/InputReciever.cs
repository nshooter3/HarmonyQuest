using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReciever : MonoBehaviour
{

    CharacterController CharController;

    private IPlayerInputManager input;

    // Start is called before the first frame update
    void Start()
    {
        CharController = GetComponent(typeof(CharacterController)) as CharacterController;
        input = ServiceLocator.instance.GetInputManager();
    }

    // Update is called once per frame
    void Update()
    {
        print("Movement: " + input.GetHorizontalMovement());
        CharController.Move(new Vector3(input.GetHorizontalMovement(), 0f,  input.GetVerticalMovement()));
    }
}
