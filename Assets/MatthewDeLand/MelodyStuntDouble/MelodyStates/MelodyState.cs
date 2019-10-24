using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MelodyState
{
    private bool IsEntering;
    protected bool AbleToExit;
    protected readonly MelodyController melodyController;

    public MelodyState(MelodyController Controller)
    {
        melodyController = Controller;
        IsEntering = true;
        AbleToExit = false;
    }
    
    protected abstract void Enter();

    public virtual void OnUpdate(float time)
    {
        if (IsEntering)
        {
            IsEntering = false;
            Enter();
        }
    }

    public virtual bool CanExit()
    {
        return AbleToExit;
    }

    public abstract MelodyState NextState();
}
