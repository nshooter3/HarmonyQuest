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

    public void PlayMusicalSFX(int scaleDegree)
    {
        musicalSFX[scaleDegree].Play();
    }

    public void PlayChord(List<FmodNote> notes)
    {
        foreach (FmodNote note in notes)
        {
            musicalSFX[note.midiValue % 12].Play();
        }
    }
}
