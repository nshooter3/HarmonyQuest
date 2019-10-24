using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelodyStateMachine
{
    readonly MelodyController Controller;
    MelodyState CurrentState;
    MelodyState NextState;


    public MelodyStateMachine(MelodyController Controller)
    {
        this.Controller = Controller;

        CurrentState = new IdleState(Controller);
    }

    public void OnUpdate(float time)
    {
        CurrentState.OnUpdate(time);
        if(CurrentState.CanExit() && CurrentState.NextState() != null)
        {
            NextState = CurrentState.NextState();
            CurrentState = NextState;
            NextState = null;
        }
    }
}
