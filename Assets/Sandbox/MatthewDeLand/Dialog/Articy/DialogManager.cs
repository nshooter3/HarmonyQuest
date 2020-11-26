using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DialogManager
{
    public static List<HQDialogSpeaker> speakers;

    public static Boolean dialoging = false;

    private static float dialogueBoxSpacing = 100;
    private static float startingAngle = 45;
    private static float degreesBetween = 180;

    private static float spacing = 150;

    public static Vector2 averagePoint;

    static DialogManager()
    {
        speakers = new List<HQDialogSpeaker>(5);
    }

    public static void PositionDialogueBoxes()
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

    private static float GetAngle(Vector2 pointA, Vector2 pointB)
    {
        var target = pointB - pointA;
        var angle = Vector2.SignedAngle(Vector2.down, target) + 180;
        return angle;
    }
}