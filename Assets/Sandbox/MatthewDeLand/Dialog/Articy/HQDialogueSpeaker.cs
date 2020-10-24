using Articy.Unity;
using HarmonyQuest;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class HQDialogueSpeaker : MonoBehaviour
{
    public ArticyRef character;
    public UITracker dialogeBoxPositioner;
    public Text dialogueText;
    public GameObject dialogueBox;
    public GameObject barkStuff;

    Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        DialogManager.speakers.Add(this);
        Debug.Log("Speaker name? " + character.GetObject().TechnicalName);
        mainCamera = ServiceLocator.instance.GetCamera();
    }

    // Update is called once per frame
    void Update()
    {
        //IsOnScreen();
    }

    public void Speak(string text)
    {
        dialogueText.text = text;
        dialogueBox.SetActive(true);
        barkStuff.SetActive(false);
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
