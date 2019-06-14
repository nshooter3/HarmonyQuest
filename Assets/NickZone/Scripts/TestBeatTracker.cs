using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBeatTracker : MonoBehaviour
{
    //Singleton for this object
    public static TestBeatTracker instance;

    //List of objects that subscribe to TestBeatTracker updates
    public List<BeatTrackerObject> beatTrackerObjects;

    public int bpm = 120;
    private float sixteenthNoteDuration;
    [HideInInspector]
    public int sixteenthNoteCount = 1;
    //Our beat based update cycle for now simply lasts 16 16th notes, which equals 4 beats, which equals 1 measure. Music, yeah!
    private int maxSixteenthNoteCount = 16;

    private float timeUntilNextSixteenthNote = 0;

    public bool playMetronome = false;
    [SerializeField]
    private AudioSource metronomeSound;

    private void Awake()
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

    // Start is called before the first frame update
    void Start()
    {
        InitBeatTrackerObjects();
        sixteenthNoteDuration = (60.0f / bpm)/4.0f;
        SixteenthNoteUpdate();
        timeUntilNextSixteenthNote = sixteenthNoteDuration;
    }

    void InitBeatTrackerObjects()
    {
        for (int i = 0; i < beatTrackerObjects.Count; i++)
        {
            beatTrackerObjects[i].beatTrackerIndex = i;
        }
    }

    void ClearNullBeatTrackerObjects()
    {
        for (int i = 0; i < beatTrackerObjects.Count; i++)
        {
            if (beatTrackerObjects[i] == null)
            {
                beatTrackerObjects.RemoveAt(i);
                i--;
            }
        }
    }

    public void RemoveBeatTrackerAtIndex(int index)
    {
        beatTrackerObjects.RemoveAt(index);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCounts();
    }

    void UpdateCounts()
    {
        timeUntilNextSixteenthNote -= Time.deltaTime;
        if (timeUntilNextSixteenthNote <= 0)
        {
            timeUntilNextSixteenthNote = sixteenthNoteDuration;
            //Once we pass our max 16th note count, reset to 1 instead of 0.
            sixteenthNoteCount = Mathf.Max((sixteenthNoteCount + 1) % (maxSixteenthNoteCount + 1), 1);
            SixteenthNoteUpdate();
        }
    }

    //Allow a little bit of wiggle room both before and after the beat for determing whether or not an action was on beat.
    public bool WasActionOnBeat(bool debug = false)
    {
        //An attack within a 32nd note before the beat will count as on beat.
        bool attackedWithinRangeBeforeBeat = (sixteenthNoteCount % 4 == 0) && (timeUntilNextSixteenthNote <= sixteenthNoteDuration / 2.0f);
        //An attack within a 32nd note after the beat will count as on beat.
        bool attackedWithinRangeAfterBeat = (sixteenthNoteCount % 4 == 1) && (timeUntilNextSixteenthNote >= sixteenthNoteDuration / 2.0f);
        if (debug)
        {
            if (attackedWithinRangeBeforeBeat)
            {
                print("GOOD TIMING, BEFORE BEAT");
            }
            else if (attackedWithinRangeAfterBeat)
            {
                print("GOOD TIMING, AFTER BEAT");
            }
            else
            {
                print("BAD TIMING, OFF BEAT");
            }
        }
        return attackedWithinRangeBeforeBeat || attackedWithinRangeAfterBeat;
    }

    void SixteenthNoteUpdate()
    {
        ClearNullBeatTrackerObjects();
        if (playMetronome && sixteenthNoteCount % 4 == 1)
        {
            metronomeSound.Play();
        }
        foreach (BeatTrackerObject beatTrackerObject in beatTrackerObjects)
        {
            beatTrackerObject.SixteenthNoteUpdate();
        }
    }

    void EighthNoteUpdate()
    {
        ClearNullBeatTrackerObjects();
        foreach (BeatTrackerObject beatTrackerObject in beatTrackerObjects)
        {
            beatTrackerObject.EighthNoteUpdate();
        }
    }

    void QuarterNoteUpdate()
    {
        ClearNullBeatTrackerObjects();
        foreach (BeatTrackerObject beatTrackerObject in beatTrackerObjects)
        {
            beatTrackerObject.QuarterNoteUpdate();
        }
    }

    void QuarterTripletNoteUpdate()
    {
        ClearNullBeatTrackerObjects();
        foreach (BeatTrackerObject beatTrackerObject in beatTrackerObjects)
        {
            beatTrackerObject.QuarterTripletNoteUpdate();
        }
    }

    void EighthTripletNoteUpdate()
    {
        ClearNullBeatTrackerObjects();
        foreach (BeatTrackerObject beatTrackerObject in beatTrackerObjects)
        {
            beatTrackerObject.EighthTripletNoteUpdate();
        }
    }
}
