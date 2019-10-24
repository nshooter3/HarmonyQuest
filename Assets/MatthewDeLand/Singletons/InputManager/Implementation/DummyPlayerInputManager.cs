using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayerInputManager : MonoBehaviour, IPlayerInputManager
{
    public bool AttackButtonDown()
    {
        return Input.GetKey(KeyCode.X);
    }

    public bool DodgeButtonDown()
    {
        return Input.GetKey(KeyCode.Z);
    }

    public float GetHorizontalMovement()
    {
        float movement = 0f;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            movement = 1.0f;
        }
        else if(Input.GetKey(KeyCode.LeftArrow))
        {
            movement = -1.0f;
        }
        return movement;
    }

    public float GetVerticalMovement()
    {
        float movement = 0f;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            movement = 1.0f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            movement = -1.0f;
        }
        return movement;
    }

    public bool HarmonyModeButtonDown()
    {
        throw new System.NotImplementedException();
    }

    public bool HealButtonDown()
    {
        throw new System.NotImplementedException();
    }

    public bool LockonButtonDown()
    {
        throw new System.NotImplementedException();
    }

    public bool ParryButtonDown()
    {
        return Input.GetKey(KeyCode.C);
    }

    // Start is called before the first frame update
    void Start()
    {
        print("Dummy Input Manager Started");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
