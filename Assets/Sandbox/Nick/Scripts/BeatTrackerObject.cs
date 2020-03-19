using UnityEngine;
abstract public class BeatTrackerObject: MonoBehaviour
{
    public int beatTrackerIndex = -1;
    public virtual void SixteenthNoteUpdate() { }
    public virtual void EighthNoteUpdate() { }
    public virtual void QuarterNoteUpdate() { }
    public virtual void EighthTripletNoteUpdate() { }
    public virtual void QuarterTripletNoteUpdate() { }
}
