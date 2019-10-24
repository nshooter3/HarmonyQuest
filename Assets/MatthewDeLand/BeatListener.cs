using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBeatListener
{
    void SixteenthNoteUpdate();
    void EighthNoteUpdate();
    void QuarterNoteUpdate();
    void EighthTripletNoteUpdate();
    void QuarterTripletNoteUpdate();
}
