using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour

{
    public static ServiceLocator instance;

    IPlayerInputManager InputManager;
       
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InputManager = GetComponent(typeof(IPlayerInputManager)) as IPlayerInputManager;
    }

    public IPlayerInputManager GetInputManager()
    {
        if(InputManager != null)
        {
            return InputManager;
        }
        else
        {
            Debug.Log("Starting a new Input Manager");
            gameObject.AddComponent(typeof(DummyPlayerInputManager));
            InputManager = GetComponent(typeof(IPlayerInputManager)) as IPlayerInputManager;
            return InputManager;
        }
    }
}
