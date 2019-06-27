using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMusicalSfxPlayer : MonoBehaviour
{
    public static TestMusicalSfxPlayer instance;

    public AudioSource[] musicalSFX;

    public float volume = 1.0f;

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
            //musicalSFX[rootNote.midiValue % 12].Play();
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
        //musicalSFX[FmodChordInterpreter.instance.GetFmodRandomNote().midiValue % 12].Play();
        float noteValue = FmodChordInterpreter.instance.GetFmodRandomNote().midiValue % 12 + 1.0f;
        FmodFacade.instance.CreateAndRunOneShotFmodEvent(eventName, volume, paramName, noteValue);
    }

    public void PlayChord(string eventName, string paramName, List<FmodNote> notes)
    {
        foreach (FmodNote note in notes)
        {
            //musicalSFX[note.midiValue % 12].Play();
            float noteValue = note.midiValue % 12 + 1.0f;
            FmodFacade.instance.CreateAndRunOneShotFmodEvent(eventName, volume, paramName, noteValue);
        }
    }
}
