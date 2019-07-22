using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestBeatTracker : MonoBehaviour
{
    //Singleton for this object
    public static TestBeatTracker instance;

    //List of objects that subscribe to TestBeatTracker updates
    public List<BeatTrackerObject> beatTrackerObjects;

    public Image debugImage;

    public float bpm = 140;
    public int beatsPerMeasure = 4;
    public float onBeatPadding = 0;
    public int sixteenthNoteCount;

    private float beatTimeDuration;
    private float beatTimer;
    public int beatCount;

    public enum OnBeatAccuracy
    {
        Great = 1,
        Good = 2,
        Miss = 3,
    }

    [SerializeField]
    private bool playMetronome = false;
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

        FmodMusicHandler.instance.AssignFunctionToOnBeatDelegate(Beat);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitBeatTrackerObjects();
        beatTimeDuration = 1.0f / (bpm / 60.0f);
        beatTimer = 0.0f;
        beatCount = 1;
    }

    public void SetTempo(float newBpm)
    {
        bpm = newBpm;
        beatTimeDuration = 1.0f / (bpm / 60.0f);
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
        if (FmodMusicHandler.instance.isMusicPlaying)
        {
            UpdateCounts();

            //Make multiplier image change colors during the "on beat" window
            if (beatTimer > beatTimeDuration - (beatTimeDuration * 0.05) ||
                beatTimer <= (beatTimeDuration * 0.05))
            {
                debugImage.color = Color.red;
            }
            else if (beatTimer > beatTimeDuration - (beatTimeDuration * onBeatPadding) /*||
           beatTimer <= (beatTimeDuration * onBeatPadding)*/)
            {
                debugImage.color = Color.yellow;
            }
            else
            {
                debugImage.color = Color.white;
            }
        }
    }

    void UpdateCounts()
    {
        beatTimer += Time.deltaTime;
        SixteenthNoteUpdate();
    }

    //FmodMusicHandler calls this during the beat callback. 
    public void Beat()
    {
        
        if (playMetronome)
        {
            metronomeSound.Play();
        }
        beatTimer = 0;
        SetTempo(FmodMusicHandler.instance.GetCurrentMusicTempo());
        beatCount = FmodMusicHandler.instance.GetCurrentBeat();
        sixteenthNoteCount = 0;
        foreach (BeatTrackerObject beatTrackerObject in beatTrackerObjects)
        {
            beatTrackerObject.SixteenthNoteUpdate();
        }
    }

    //Allow a little bit of wiggle room both before and after the beat for determining whether or not an action was on beat.
    public OnBeatAccuracy WasActionOnBeat(bool debug = false)
    {
        //Full onBeatPadding range for good on beat
        bool attackedWithinRangeBeforeBeatGood = beatTimer > beatTimeDuration - (beatTimeDuration * onBeatPadding);
        bool attackedWithinRangeAfterBeatGood = beatTimer <= (beatTimeDuration * onBeatPadding);

        //Half onBeatPadding range for great on beat. This is half the window of good on beat.
        bool attackedWithinRangeBeforeBeatGreat = beatTimer > beatTimeDuration - (beatTimeDuration * (onBeatPadding / 2.0f));
        bool attackedWithinRangeAfterBeatGreat = beatTimer <= (beatTimeDuration * (onBeatPadding/2.0f));

        if (attackedWithinRangeBeforeBeatGreat || attackedWithinRangeAfterBeatGreat)
        {
            return OnBeatAccuracy.Great;
        }
        else if (attackedWithinRangeBeforeBeatGood || attackedWithinRangeAfterBeatGood)
        {
            return OnBeatAccuracy.Good;
        }
        return OnBeatAccuracy.Miss;
    }


    void SixteenthNoteUpdate()
    {
        if (sixteenthNoteCount < 3)
        {
            ClearNullBeatTrackerObjects();
            int newSixteenthNoteCount = (int)(beatTimer / (beatTimeDuration / 4.0f));
            if (sixteenthNoteCount != newSixteenthNoteCount)
            {
                sixteenthNoteCount = newSixteenthNoteCount;
                //print("@@@ sixteenth notes" + sixteenthNoteCount);
                foreach (BeatTrackerObject beatTrackerObject in beatTrackerObjects)
                {
                    beatTrackerObject.SixteenthNoteUpdate();
                }
            }
        }
    }

    /*
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
    */
}