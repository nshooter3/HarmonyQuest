using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMusicalSfxPlayer : MonoBehaviour
{
    public static TestMusicalSfxPlayer instance;

    public AudioSource[] musicalSFX;

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

    public void PlayRootNote()
    {
        FmodNote rootNote = FmodChordInterpreter.instance.GetFmodRootNote();
        if (rootNote != null)
        {
            musicalSFX[rootNote.midiValue % 12].Play();
        }
        else
        {
            Debug.LogWarning("No root note found in current chord. Playing nothing.");
        }
    }

    public void PlayRandomNote()
    {
        musicalSFX[FmodChordInterpreter.instance.GetFmodRandomNote().midiValue % 12].Play();
    }

    public void PlayChord(List<FmodNote> notes)
    {
        foreach (FmodNote note in notes)
        {
            musicalSFX[note.midiValue % 12].Play();
        }
    }
}
