using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmodMusicalSfxPlayer : MonoBehaviour
{
    public static FmodMusicalSfxPlayer instance;

    public float volume = 1.0f;

    public bool resetNoteIndexOnNewChord = false;

    //Determines what this object does when it is told to play a musical SFX
    public enum SfxMode
    {
        root,
        upwards,
        downwards,
        upThenDown,
        downThenUp,
        random,
        chord,
        glissUp,
        glissDown,
    };

    public SfxMode sfxMode;

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

    public void PlayRootNote(string eventName, string paramName)
    {
        FmodNote rootNote = FmodChordInterpreter.instance.GetFmodRootNote();
        if (rootNote != null)
        {
            float noteValue = rootNote.midiValue % 12 + 1.0f;
            FmodFacade.instance.CreateAndRunOneShotFmodEvent(eventName, volume, paramName, noteValue);
        }
        else
        {
            Debug.LogWarning("No root note found in current chord. Playing nothing.");
        }
    }

    public void PlayRandomNote(string eventName, string paramName)
    {
        float noteValue = FmodChordInterpreter.instance.GetFmodRandomNote().midiValue % 12 + 1.0f;
        FmodFacade.instance.CreateAndRunOneShotFmodEvent(eventName, volume, paramName, noteValue);
    }

    public void PlayChord(string eventName, string paramName, List<FmodNote> notes)
    {
        foreach (FmodNote note in notes)
        {
            float noteValue = note.midiValue % 12 + 1.0f;
            FmodFacade.instance.CreateAndRunOneShotFmodEvent(eventName, volume, paramName, noteValue);
        }
    }
}
