using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BeatListenerEntity : Entity, IBeatListener
{
    

    // Start is called before the first frame update
    new void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EighthNoteUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void EighthTripletNoteUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void QuarterNoteUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void QuarterTripletNoteUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void SixteenthNoteUpdate()
    {
        throw new System.NotImplementedException();
    }
}
