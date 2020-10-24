using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DialogManager
{
    public static List<HQDialogueSpeaker> speakers;

    private static float  dialogueBoxSpacing = 100;
    private static float startingAngle = 45;
    private static float degreesBetween = 180;

    private static float spacing = 150;

    public static Vector2 averagePoint;

    static DialogManager()
    {
        speakers = new List<HQDialogueSpeaker>(25);
    }

    public static void PositionDialogueBoxes()
    {
        //Get Average X and Y of Speakers OnScreen
        Vector2 average = new Vector2();
        int countOnScreen = 0;
        foreach (HQDialogueSpeaker speaker in speakers)
        {
            
            if (speaker.IsOnScreen())
            {
                Vector2 screenPos = speaker.GetScreenPostion();
                average += screenPos;
                countOnScreen++;
            }            
        }
        average /= countOnScreen;
        averagePoint = average;

        float[] anglesFromAverage = new float[speakers.Count];
        for (int i = 0;  i < anglesFromAverage.Length; i++)
        {
           anglesFromAverage[i] = GetAngle(average, speakers[i].GetScreenPostion());
        }

        //Start with whatever is in the top left. Change dialog box offsets then move ever how many angles
        float angleBetweenDialogBoxes = 360f / speakers.Count;
        bool[] dialogueBoxPositioned = new bool[speakers.Count];
        for(int i = 0; i < dialogueBoxPositioned.Length; i++)
        {
            dialogueBoxPositioned[i] = false;
        }
        for (int i = 0; i < speakers.Count; i++)
        {
            int  closestIndex = 0;
            float closestDeltaAngle = 360f;
            float targetAngle = (startingAngle + angleBetweenDialogBoxes * i);
            for (int j = 0; j < anglesFromAverage.Length; j++)
            {
                if(Math.Abs(anglesFromAverage[j] - targetAngle) < closestDeltaAngle)
                {
                    closestIndex = j;
                    closestDeltaAngle = anglesFromAverage[j];
                }
            }
            float newXOffset = (float)Math.Cos(targetAngle * (Math.PI / 180f)) * dialogueBoxSpacing;
            float newYOffset = (float)Math.Sin(targetAngle * (Math.PI / 180f)) * dialogueBoxSpacing;
            /*
            if(anglesFromAverage[closestIndex] > 90 && anglesFromAverage[closestIndex] < 270)
            {
                newYOffset *= -1;
            }
            if (anglesFromAverage[closestIndex] > 180 && anglesFromAverage[closestIndex] < 360)
            {
                newXOffset *= -1;
            }
            /*
            Debug.Log("Target Angle = " + targetAngle);
            Debug.Log("Closest Index = " + closestIndex);
            Debug.Log("Target Angle = " + targetAngle);
            */
            //speakers[closestIndex].dialogeBoxPositioner.SetOffsets(newXOffset, newYOffset);
        }
    }


    public static void PositionDialogueBoxes2()
    {
        //Get Average X and Y of Speakers OnScreen
        Vector2 average = new Vector2();

        foreach (HQDialogueSpeaker speaker in speakers)
        {
            if (speaker.IsOnScreen())
            {
                Vector2 screenPos = speaker.GetScreenPostion();
                average += screenPos;
            }
        }
        average /= speakers.Count;

        foreach (HQDialogueSpeaker speaker in speakers)
        {
            if (speaker.GetScreenPostion().x >= average.x)
            {
                if(speaker.GetScreenPostion().y >= average.y)
                {
                    speaker.dialogeBoxPositioner.SetOffsets(spacing, spacing);
                }
                else
                {
                    speaker.dialogeBoxPositioner.SetOffsets(-spacing, spacing);
                }
            }
            else
            {
                if (speaker.GetScreenPostion().y >= average.y)
                {
                    speaker.dialogeBoxPositioner.SetOffsets(spacing, -spacing);
                }
                else
                {
                    speaker.dialogeBoxPositioner.SetOffsets(-spacing, -spacing);
                }
            }
        }

       
    }


    public static void Speak(String speakerTechnicalName, String text)
    {
        Debug.Log("Speakers is: " + speakerTechnicalName);
        //HQDialogueSpeaker speaker;
        foreach(HQDialogueSpeaker speaker in speakers)
        {
            if(speaker.character.GetObject().TechnicalName == speakerTechnicalName)
            {
                speaker.Speak(text);
            }
        }
    }

    private static float GetAngle(Vector2 pointA, Vector2 pointB)
    {
        /*
        var target = pointB - pointA;
        var angle = Vector2.Angle(pointA, pointB);
        var orientation = Mathf.Sign(pointA.x * target.y - pointA.y * target.x);
        return (360 - orientation * angle) % 360;
        */
        var target = pointB - pointA;
        var angle = Vector2.SignedAngle(Vector2.down, target)+180;
        return angle;
    }

    public static float GetAngle2(Vector3 from, Vector3 to, Vector3 normal)
    {
        // angle in [0,180]
        float angle = Vector3.Angle(from, to);
        float sign = Mathf.Sign(Vector3.Dot(normal, Vector3.Cross(from, to)));
        return (angle * sign)+180;
    }
}
