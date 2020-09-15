using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DialogManager
{
    public static List<HQDialogueSpeaker> speakers;

    private static float  dialogueBoxSpacing = 300;
    private static float startingAngle = 45;
    private static float degreesBetween = 180;

    static DialogManager()
    {
        speakers = new List<HQDialogueSpeaker>(25);
    }

    public static void PositionDialogueBoxes()
    {
        //Get Average X and Y of Speakers OnScreen
        Vector2 average = new Vector2();
        foreach(HQDialogueSpeaker speaker in speakers)
        {
            if (speaker.IsOnScreen())
            {
                Vector2 screenPos = speaker.GetScreenPostion();
                Debug.Log("Position " + screenPos.ToString());
                average += screenPos;
            }            
        }
        average /= speakers.Count;
        Debug.Log("average " + average.ToString());

        float[] anglesFromAverage = new float[speakers.Count];
        for (int i = 0;  i < anglesFromAverage.Length; i++)
        {
           anglesFromAverage[i] = GetAngle2(average, speakers[i].GetScreenPostion(), average.Normalize();
           Debug.Log("Angles: "+anglesFromAverage[i]);
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
            if(anglesFromAverage[closestIndex] > 90 && anglesFromAverage[closestIndex] < 270)
            {
                newYOffset *= -1;
            }
            if (anglesFromAverage[closestIndex] > 180 && anglesFromAverage[closestIndex] < 360)
            {
                newXOffset *= -1;
            }
            Debug.Log("Target Angle = " + targetAngle);
            Debug.Log("Closest Index = " + closestIndex);
            Debug.Log("Target Angle = " + targetAngle);
            speakers[closestIndex].dialogeBoxPositioner.SetOffsets(newXOffset, newYOffset);
        }
    }

    private static float GetAngle(Vector2 pointA, Vector2 pointB)
    {
        var target = pointB - pointA;
        var angle = Vector2.Angle(pointA, pointB);
        var orientation = Mathf.Sign(pointA.x * target.y - pointA.y * target.x);
        return (360 - orientation * angle) % 360;
    }

    public static float GetAngle2(Vector3 from, Vector3 to, Vector3 normal)
    {
        // angle in [0,180]
        float angle = Vector3.Angle(from, to);
        float sign = Mathf.Sign(Vector3.Dot(normal, Vector3.Cross(from, to)));
        return (angle * sign)+180;
    }
}
