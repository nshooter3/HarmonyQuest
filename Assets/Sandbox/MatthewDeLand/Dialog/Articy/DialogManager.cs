using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DialogManager
{
    public static List<HQDialogSpeaker> speakers;

    public static bool dialoging = false;

    private static float spacing = 150;

    public static Vector2 averagePoint;

    static DialogManager()
    {
        speakers = new List<HQDialogSpeaker>(5);
    }

    public static void PositionDialogBoxes()
    {
        //Get Average X and Y of Speakers OnScreen
        Vector2 average = new Vector2();
        foreach (HQDialogSpeaker speaker in speakers)
        {
            if (speaker.IsOnScreen())
            {
                Vector2 screenPos = speaker.GetScreenPostion();
                average += screenPos;
            }
        }
        average /= speakers.Count;

        foreach (HQDialogSpeaker speaker in speakers)
        {
            if (speaker.GetScreenPostion().x >= average.x)
            {
                if (speaker.GetScreenPostion().y >= average.y)
                {
                    speaker.dialogView.mainTracker.SetOffsets(spacing, spacing);
                }
                else
                {
                    speaker.dialogView.mainTracker.SetOffsets(-spacing, spacing);
                    
                }
            }
            else
            {
                if (speaker.GetScreenPostion().y >= average.y)
                {
                    speaker.dialogView.mainTracker.SetOffsets(spacing, -spacing);
                }
                else
                {
                    speaker.dialogView.mainTracker.SetOffsets(-spacing, -spacing);
                }
            }
        }


    }

    public static void Speak(String speakerTechnicalName, String text)
    {
        foreach (HQDialogSpeaker speaker in speakers)
        {
            if (speaker.character.GetObject().TechnicalName == speakerTechnicalName)
            {
                speaker.Speak(text);
            }
        }
    }
}