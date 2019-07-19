using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FmodSfxPlayer : MonoBehaviour
{
    public string sfxEventName = "";
    public float sfxEventVolume = 1.0f;
    public string musicalSfxParamName = "";

    //Used in the GlissUp and GlissDown behaviors to determine spacing between notes
    public float glissTimeBetweenNotes = 0.1f;

    //Determines what this object does when it is told to play a musical SFX
    public enum TonalSfxMode
    {
        //No Tonal Sfx behavior
        None,

        //Single tonal note behaviors
        Root,
        Upwards,
        Downwards,
        UpThenDown,
        DownThenUp,
        Random,

        //Multiple tonal notes behaviors
        Chord,
        GlissUp,
        GlissDown,
    };

    public TonalSfxMode sfxMode;

    //Determines what this object's index does upon a chord change
    public enum IndexBehaviorOnNewChord
    {
        DoNothing,
        ResetNoteToInitIndex,
        ResetIndexToLastIndexPlayed,
    };

    public IndexBehaviorOnNewChord indexBehaviorOnNewChord;

    private int noteIndex = 0;
    private int lastPlayedNoteIndex = 0;
    private int noteDirection;
    private List<FmodNote> notesInChord;

    private bool firstPlayRequest = true;

    private string octaveParamName = "global_octave";

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
            case TonalSfxMode.UpThenDown:
                noteDirection = 1;
                break;
            case TonalSfxMode.DownThenUp:
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
            case TonalSfxMode.UpThenDown:
                noteDirection = 1;
                break;
            case TonalSfxMode.DownThenUp:
                noteDirection = -1;
                noteIndex = chord.Count - 1;
                break;
            case TonalSfxMode.Downwards:
                noteIndex = chord.Count - 1;
                break;
            case TonalSfxMode.GlissDown:
                noteIndex = chord.Count - 1;
                break;
        }
    }

    public void Play(FmodParamData[] extraParams = null)
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
            case TonalSfxMode.None:
                PlayNonTonalNote(extraParams);
                break;
            case TonalSfxMode.Root:
                PlayRootNote(extraParams);
                break;
            case TonalSfxMode.Upwards:
                PlayNoteUpdwards(extraParams);
                break;
            case TonalSfxMode.Downwards:
                PlayNoteDownwards(extraParams);
                break;
            case TonalSfxMode.UpThenDown:
                PlayNoteUpThenDown(extraParams);
                break;
            case TonalSfxMode.DownThenUp:
                PlayNoteDownThenUp(extraParams);
                break;
            case TonalSfxMode.Random:
                PlayRandomNote(extraParams);
                break;
            case TonalSfxMode.Chord:
                PlayChord(extraParams);
                break;
            case TonalSfxMode.GlissUp:
                PlayGlissUp(extraParams);
                break;
            case TonalSfxMode.GlissDown:
                PlayGlissDown(extraParams);
                break;
        }
    }

    private void PlayNonTonalNote(FmodParamData[] extraParams = null)
    {
        FmodFacade.instance.CreateAndRunOneShotFmodEvent(sfxEventName, sfxEventVolume, extraParams);
    }

    private void PlayRootNote(FmodParamData[] extraParams = null)
    {
        FmodNote rootNote = FmodChordInterpreter.instance.GetFmodRootNote();
        if (rootNote != null)
        {
            float noteValue = ConvertMidiValueToFmodParamValue(rootNote.midiValue);
            float noteOctave = rootNote.octave;

            FmodParamData[] paramData = GenerateParamData(musicalSfxParamName, noteValue, octaveParamName, noteOctave, extraParams);
            FmodFacade.instance.CreateAndRunOneShotFmodEvent(sfxEventName, sfxEventVolume, paramData);
        }
        else
        {
            Debug.LogWarning("No root note found in current chord. Playing nothing.");
        }
    }

    private void PlayNoteUpdwards(FmodParamData[] extraParams = null)
    {
        PlayNoteAtIndex(noteIndex, extraParams);
        noteIndex = (noteIndex + 1) % notesInChord.Count;
    }

    private void PlayNoteDownwards(FmodParamData[] extraParams = null)
    {
        PlayNoteAtIndex(noteIndex, extraParams);
        noteIndex = noteIndex - 1;
        if (noteIndex < 0)
        {
            noteIndex = notesInChord.Count - 1;
        }
    }

    private void PlayNoteUpThenDown(FmodParamData[] extraParams = null)
    {
        //Debug.Log("PLAY CHORD NOTE: " + noteIndex + ", WITH MIDI VALUE " + notesInChord[noteIndex].midiValue);
        PlayNoteAtIndex(noteIndex, extraParams);
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

    private void PlayNoteDownThenUp(FmodParamData[] extraParams = null)
    {
        //Simply use PlayNoteUpThenDown logic, since the only difference is which note we start on.
        PlayNoteUpThenDown(extraParams);
    }

    private void PlayNoteAtIndex(int index, FmodParamData[] extraParams = null)
    {
        float noteValue = ConvertMidiValueToFmodParamValue(FmodChordInterpreter.instance.GetFmodNoteAtIndex(index).midiValue);
        float noteOctave = FmodChordInterpreter.instance.GetFmodNoteAtIndex(index).octave;

        FmodParamData[] paramData = GenerateParamData(musicalSfxParamName, noteValue, octaveParamName, noteOctave, extraParams);
        FmodFacade.instance.CreateAndRunOneShotFmodEvent(sfxEventName, sfxEventVolume, paramData);
        lastPlayedNoteIndex = index;
    }

    private void PlayRandomNote(FmodParamData[] extraParams = null)
    {
        float noteValue = ConvertMidiValueToFmodParamValue(FmodChordInterpreter.instance.GetFmodRandomNote().midiValue);
        float noteOctave = FmodChordInterpreter.instance.GetFmodRandomNote().octave;

        FmodParamData[] paramData = GenerateParamData(musicalSfxParamName, noteValue, octaveParamName, noteOctave, extraParams);
        FmodFacade.instance.CreateAndRunOneShotFmodEvent(sfxEventName, sfxEventVolume, paramData);
    }

    private void PlayChord(FmodParamData[] extraParams = null)
    {
        foreach (FmodNote note in notesInChord)
        {
            float noteValue = ConvertMidiValueToFmodParamValue(note.midiValue);
            float noteOctave = note.octave;

            FmodParamData[] paramData = GenerateParamData(musicalSfxParamName, noteValue, octaveParamName, noteOctave, extraParams);
            FmodFacade.instance.CreateAndRunOneShotFmodEvent(sfxEventName, sfxEventVolume, paramData);
        }
    }

    private void PlayGlissUp(FmodParamData[] extraParams = null)
    {
        for(int i = 0; i < notesInChord.Count; i ++)
        {
            //Debug.Log("PLAY CHORD NOTE: " + i + ", WITH MIDI VALUE " + notesInChord[i].midiValue);
            float noteValue = ConvertMidiValueToFmodParamValue(notesInChord[i].midiValue);
            float noteOctave = notesInChord[i].octave;
            float delay = glissTimeBetweenNotes * i;

            FmodParamData[] paramData = GenerateParamData(musicalSfxParamName, noteValue, octaveParamName, noteOctave, extraParams);
            IEnumerator coroutine = PlayNoteDelayed(delay, sfxEventName, sfxEventVolume, paramData);
            StartCoroutine(coroutine);
        }
    }

    private void PlayGlissDown(FmodParamData[] extraParams = null)
    {
        for (int i = notesInChord.Count - 1; i >= 0; i--)
        {
            float noteValue = ConvertMidiValueToFmodParamValue(notesInChord[i].midiValue);
            float noteOctave = notesInChord[i].octave;
            float delay = glissTimeBetweenNotes * ((notesInChord.Count - 1) - i);

            FmodParamData[] paramData = GenerateParamData(musicalSfxParamName, noteValue, octaveParamName, noteOctave, extraParams);
            IEnumerator coroutine = PlayNoteDelayed(delay, sfxEventName, sfxEventVolume, paramData);
            StartCoroutine(coroutine);
        }
    }

    IEnumerator PlayNoteDelayed(float delay, string sfxEventName, float sfxEventVolume, FmodParamData[] paramData)
    {
        yield return new WaitForSeconds(delay);
        FmodFacade.instance.CreateAndRunOneShotFmodEvent(sfxEventName, sfxEventVolume, paramData);
    }

    private float ConvertMidiValueToFmodParamValue(int midiValue)
    {
        return midiValue % 12 + 1.0f;
    }

    private FmodParamData[] GenerateParamData(string musicalSfxParamName, float noteValue, string octaveParamName, float noteOctave, FmodParamData[] extraParamData = null)
    {
        FmodParamData[] paramData = { new FmodParamData(musicalSfxParamName, noteValue), new FmodParamData(octaveParamName, noteOctave) };
        if (extraParamData != null)
        {
            paramData = paramData.Concat(extraParamData).ToArray();
        }
        return paramData;
    }
}
