using HarmonyQuest;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class HQDialogueSpeaker : MonoBehaviour
{

    public UITracker dialogeBoxPositioner;

    Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        DialogManager.speakers.Add(this);
        mainCamera = ServiceLocator.instance.GetCamera();
    }

    // Update is called once per frame
    void Update()
    {
        //IsOnScreen();
    }

    public bool IsOnScreen()
    {
        Vector2 screenPoint = GetScreenPostion();
        if (screenPoint.x > mainCamera.pixelWidth || screenPoint.x < 0 || screenPoint.y > mainCamera.pixelHeight || screenPoint.y < 0)
        {
            return false;
        }
        else
        {
            return true;
        }        
    }

    public Vector2 GetScreenPostion()
    {
        return mainCamera.WorldToScreenPoint(transform.position);
    }
}
