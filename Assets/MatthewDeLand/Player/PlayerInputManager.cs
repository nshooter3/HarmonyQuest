using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInputManager: MonoBehaviour
{
    public static PlayerInputManager instance;

    void Awake()
    {
        OnAwake();
    }

    virtual public void OnAwake()
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

    abstract public float GetHorizontalMovement();

    abstract public float GetVerticalMovement();

    abstract public bool AttackButtonDown();

    abstract public bool ParryButtonDown();

    abstract public bool DodgeButtonDown();

    abstract public bool HarmonyModeButtonDown();

    abstract public bool HealButtonDown();

    abstract public bool LockonButtonDown();
}
