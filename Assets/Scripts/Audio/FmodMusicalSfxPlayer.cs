using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmodMusicalSfxPlayer : MonoBehaviour
{
    public string sfxEventName = "";
    public string sfxParamName = "";
    public float sfxEventVolume = 1.0f;

    public float glissDelay = 0.1f;

    private int noteIndex = 0;
    private int lastPlayedNoteIndex = 0;
    private int noteDirection;
    private List<FmodNote> notesInChord;

    private bool firstPlayRequest = true;

    private string octaveParamName = "global_octave";

    //Determines what this object does when it is told to play a musical SFX
    public enum SfxMode
    {
        //Single note behaviors
        Root,
        Upwards,
        Downwards,
        UpThenDown,
        DownThenUp,
        Random,

        //Multiple note behaviors
        Chord,
        GlissUp,
        GlissDown,
    };

    public SfxMode sfxMode;

    //Determines what this object's index does upon a chord change
    public enum IndexBehaviorOnNewChord
    {
        DoNothing,
        ResetNoteToInitIndex,
        ResetIndexToLastIndexPlayed,
    };

    public IndexBehaviorOnNewChord indexBehaviorOnNewChord;

    private void Awake()
    {
        if (indexBehaviorOnNewChord == IndexBehaviorOnNewChord.ResetNoteToInitIndex)
        {
            FmodMusicHandler.instance.AssignFunctionToOnChordMarkerDelegate(ResetNoteToInitIndex);
        }
        else if (indexBehaviorOnNewChord == IndexBehaviorOnNewChord.ResetIndexToLastIndexPlayed)
        {
            FmodMusicHandler.instance.AssignFunctionToOnChordMarkerDelegate(ResetNoteToLastPlayedIndex);
        }
    }

    private void Start()
    {
        switch (sfxMode)
        {
            case SfxMode.UpThenDown:
                noteDirection = 1;
                break;
            case SfxMode.DownThenUp:
                noteDirection = -1;
                break;
        }
    }

    private void ResetNoteToInitIndex(List<FmodNote> chord)
    {
        InitNoteIndexAndDirection(chord);
    }

    private void ResetNoteToLastPlayedIndex(List<FmodNote> chord)
    {
        noteIndex = lastPlayedNoteIndex;
    }

    private void InitNoteIndexAndDirection(List<FmodNote> chord)
    {
        noteIndex = 0;
        switch (sfxMode)
        {
            case SfxMode.UpThenDown:
                noteDirection = 1;
                break;
            case SfxMode.DownThenUp:
                noteDirection = -1;
                noteIndex = chord.Count - 1;
                break;
            case SfxMode.Downwards:
                noteIndex = chord.Count - 1;
                break;
            case SfxMode.GlissDown:
                noteIndex = chord.Count - 1;
                break;
        }
    }

    public void Play()
    {
        notesInChord = FmodChordInterpreter.instance.GetFmodChord();

        //If this is the first time we're playing this note, ensure that we start our note progression on the right note, going in the right direction.
        if (firstPlayRequest)
        {
            InitNoteIndexAndDirection(notesInChord);
            firstPlayRequest = false;
        }

        switch (sfxMode)
        {
            case SfxMode.Root:
                PlayRootNote();
                break;
            case SfxMode.Upwards:
                PlayNoteUpdwards();
                break;
            case SfxMode.Downwards:
                PlayNoteDownwards();
                break;
            case SfxMode.UpThenDown:
                PlayNoteUpThenDown();
                break;
            case SfxMode.DownThenUp:
                PlayNoteDownThenUp();
                break;
            case SfxMode.Random:
                PlayRandomNote();
                break;
            case SfxMode.Chord:
                PlayChord();
                break;
            case SfxMode.GlissUp:
                PlayGlissUp();
                break;
            case SfxMode.GlissDown:
                PlayGlissDown();
                break;
        }
    }

    private void PlayRootNote()
    {
        FmodNote rootNote = FmodChordInterpreter.instance.GetFmodRootNote();
        if (rootNote != null)
        {
            float noteValue = ConvertMidiValueToFmodParamValue(rootNote.midiValue);
            float noteOctave = rootNote.octave;
            //Debug.Log("NOTE TO PLAY: " + rootNote.note + noteOctave);
            FmodFacade.instance.CreateAndRunOneShotFmodEvent(sfxEventName, sfxEventVolume, sfxParamName, octaveParamName, noteValue, noteOctave);
        }
        else
        {
            Debug.LogWarning("No root note found in current chord. Playing nothing.");
        }
    }

    private void PlayNoteUpdwards()
    {
        PlayNoteAtIndex(noteIndex);
        noteIndex = (noteIndex + 1) % notesInChord.Count;
    }

    private void PlayNoteDownwards()
    {
        PlayNoteAtIndex(noteIndex);
        noteIndex = noteIndex - 1;
        if (noteIndex < 0)
        {
            noteIndex = notesInChord.Count - 1;
        }
    }

    private void PlayNoteUpThenDown()
    {
        //Debug.Log("PLAY CHORD NOTE: " + noteIndex + ", WITH MIDI VALUE " + notesInChord[noteIndex].midiValue);
        PlayNoteAtIndex(noteIndex);
        noteIndex = noteIndex + noteDirection;

        if (noteIndex >= notesInChord.Count - 1)
        {
            noteIndex = notesInChord.Count - 1;
            noteDirection = -1;
        }
        else if (noteIndex <= 0)
        {
            noteIndex = 0;
            noteDirection = 1;
        }
    }

    private void PlayNoteDownThenUp()
    {
        //Simply use PlayNoteUpThenDown logic, since the only difference is which note we start on.
        PlayNoteUpThenDown();
    }

    private void PlayNoteAtIndex(int index)
    {
        float noteValue = ConvertMidiValueToFmodParamValue(FmodChordInterpreter.instance.GetFmodNoteAtIndex(index).midiValue);
        float noteOctave = FmodChordInterpreter.instance.GetFmodNoteAtIndex(index).octave;
        //Debug.Log("NOTE TO PLAY: " + FmodChordInterpreter.instance.GetFmodNoteAtIndex(index).note + FmodChordInterpreter.instance.GetFmodNoteAtIndex(index).octave);
        FmodFacade.instance.CreateAndRunOneShotFmodEvent(sfxEventName, sfxEventVolume, sfxParamName, octaveParamName, noteValue, noteOctave);
        lastPlayedNoteIndex = index;
    }

    private void PlayRandomNote()
    {
        float noteValue = ConvertMidiValueToFmodParamValue(FmodChordInterpreter.instance.GetFmodRandomNote().midiValue);
        float noteOctave = FmodChordInterpreter.instance.GetFmodRandomNote().octave;
        FmodFacade.instance.CreateAndRunOneShotFmodEvent(sfxEventName, sfxEventVolume, sfxParamName, octaveParamName, noteValue, noteOctave);
    }

    private void PlayChord()
    {
        foreach (FmodNote note in notesInChord)
        {
            float noteValue = ConvertMidiValueToFmodParamValue(note.midiValue);
            float noteOctave = note.octave;
            FmodFacade.instance.CreateAndRunOneShotFmodEvent(sfxEventName, sfxEventVolume, sfxParamName, octaveParamName, noteValue, noteOctave);
        }
    }

    private void PlayGlissUp()
    {
        for(int i = 0; i < notesInChord.Count; i ++)
        {
            //Debug.Log("PLAY CHORD NOTE: " + i + ", WITH MIDI VALUE " + notesInChord[i].midiValue);
            float noteValue = ConvertMidiValueToFmodParamValue(notesInChord[i].midiValue);
            float noteOctave = notesInChord[i].octave;
            float delay = glissDelay * i;
            IEnumerator coroutine = PlayNoteDelayed(delay, sfxEventName, sfxEventVolume, sfxParamName, octaveParamName, noteValue, noteOctave);
            StartCoroutine(coroutine);
        }
    }

    private void PlayGlissDown()
    {
        for (int i = notesInChord.Count - 1; i >= 0; i--)
        {
            float noteValue = ConvertMidiValueToFmodParamValue(notesInChord[i].midiValue);
            float noteOctave = notesInChord[i].octave;
            float delay = glissDelay * ((notesInChord.Count - 1) - i);
            IEnumerator coroutine = PlayNoteDelayed(delay, sfxEventName, sfxEventVolume, sfxParamName, octaveParamName, noteValue, noteOctave);
            StartCoroutine(coroutine);
        }
    }

    IEnumerator PlayNoteDelayed(float delay, string sfxEventName, float sfxEventVolume, string sfxParamName, string octaveParamName, float noteValue, float noteOctave)
    {
        yield return new WaitForSeconds(delay);
        FmodFacade.instance.CreateAndRunOneShotFmodEvent(sfxEventName, sfxEventVolume, sfxParamName, octaveParamName, noteValue, noteOctave);
    }

    private float ConvertMidiValueToFmodParamValue(int midiValue)
    {
        return midiValue % 12 + 1.0f;
    }
}
