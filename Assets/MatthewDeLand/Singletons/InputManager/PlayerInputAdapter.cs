using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputAdapter : MonoBehaviour, IPlayerInputManager
{
    public IPlayerInputManager implementation;

    public static PlayerInputAdapter instance;

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

    public bool AttackButtonDown()
    {
        return implementation.AttackButtonDown();
    }

    public bool DodgeButtonDown()
    {
        return implementation.DodgeButtonDown();
    }

    public float GetHorizontalMovement()
    {
        return implementation.GetHorizontalMovement();
    }

    public float GetVerticalMovement()
    {
        return implementation.GetVerticalMovement();
    }

    public bool HarmonyModeButtonDown()
    {
        return implementation.HarmonyModeButtonDown();
    }

    public bool HealButtonDown()
    {
        return implementation.HealButtonDown();
    }

    public bool LockonButtonDown()
    {
        return implementation.LockonButtonDown();
    }

    public bool ParryButtonDown()
    {
        return implementation.ParryButtonDown();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
