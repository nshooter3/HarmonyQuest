using Articy.Unity;
using Articy.Unity.Interfaces;
using GameManager;
using HarmonyQuest;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : ManageableObject, IArticyFlowPlayerCallbacks
{
    private List<HQDialogSpeaker> speakers;
    private const float spacing = 150;
    private ArticyFlowPlayer flowPlayer;
    private ArticyRef proximalDialog;
    private IList<Branch> dialogOptions;
    private Branch dialog;
    private bool awaitingResponse = false;

    public bool inDialog = false;
  
  

    public GameObject dialogOptionHolder;

    public DialogController()
    {
        speakers = new List<HQDialogSpeaker>(5);
    }

    public bool RegisterSpeaker(HQDialogSpeaker speaker)
    {
        if (!speakers.Contains(speaker))
        {
            speakers.Add(speaker);
            return true;
        }
        return false;
    }

    public void SetDialog(ArticyRef dialogRef)
    {
        if (dialogRef.GetObject() != null)
        {
            flowPlayer.StartOn = dialogRef.GetObject();
        }        
    }

    public void EnterSpeakerZone(HQDialogSpeaker speaker)
    {
        if(proximalDialog != speaker.DialogReference)
        {
            proximalDialog = speaker.DialogReference;
        }
    }


    public void ExitSpeakerZone(HQDialogSpeaker speaker)
    {
        if (proximalDialog == speaker.DialogReference)
        {
            proximalDialog = null;
        }
    }



    public void Speak(string speakerTechnicalName, string text)
    {
        foreach (HQDialogSpeaker speaker in speakers)
        {
            if (speaker.character.GetObject().TechnicalName == speakerTechnicalName)
            {
                speaker.Speak(text);
            }
            else
            {
                speaker.ShutUp();
            }
        }
    }

    public void EndDialog()
    {
        foreach (HQDialogSpeaker speaker in speakers)
        {
            speaker.ShutUp();
        }
    }

    public void PositionDialogBoxes()
    {
        //Get Average X and Y of Speakers in screen coordinates
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

        //Pick a quadrant to place the dialog box based on where the speaker is
        //in relation to the average point
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

    public void OnBranchesUpdated(IList<Branch> aBranches)
    {

        dialogOptions = aBranches;

        if (aBranches.Count > 1)
        {
            dialogOptionHolder.SetActive(true);
            awaitingResponse = true;
            Text[] options = dialogOptionHolder.GetComponentsInChildren<Text>();
            int index = 0;
            foreach (Branch branch in aBranches)
            {
                var menuText = branch.Target as IObjectWithMenuText;
                if (menuText != null) 
                {
                    options[index].text = menuText.MenuText;
                }
                index++;
            }
            while (index < options.Length)
            {
                options[index].gameObject.transform.parent.gameObject.SetActive(false);
                index++;
            }            
        }
        else if (aBranches.Count == 1)
        {
            dialog = aBranches[0];
            dialogOptionHolder.SetActive(false);
            var output = dialog.Target as IOutputPin;
            if (output != null)
            {
                ServiceLocator.instance.GetDialogController().EndDialog();
                flowPlayer.Play(dialog);
                flowPlayer.FinishCurrentPausedObject();
                flowPlayer.StartOn = null;
                inDialog = false;
            }
            else
            {
                var text = dialog.Target as IObjectWithText;
                var speaker = dialog.Target as IObjectWithSpeaker;
                if (speaker != null)
                {
                    ServiceLocator.instance.GetDialogController().Speak(speaker.Speaker.TechnicalName, text.Text);
                }                    
            }
            
        }
    }

    public void OnFlowPlayerPaused(IFlowObject aObject){}

    public override void OnAwake() 
    {
        flowPlayer = FindObjectOfType<ArticyFlowPlayer>();
        
    }

    public override void OnStart() 
    {
        Button[] buttons = dialogOptionHolder.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() => DialogOptionSelected(0));
        buttons[1].onClick.AddListener(() => DialogOptionSelected(1));
        buttons[2].onClick.AddListener(() => DialogOptionSelected(2));
    }

    public override void OnUpdate()
    {
        if (ServiceLocator.instance.GetInputManager().InteractButtonDown() && !awaitingResponse)
        {
            if (inDialog)
            {
              flowPlayer.Play(dialog);
              flowPlayer.FinishCurrentPausedObject();
            }
            else
            {
                if (proximalDialog!= null && proximalDialog.HasReference)
                {
                    inDialog = true;
                    SetDialog(proximalDialog);
                    proximalDialog = null;
                }                
            }            
        }
    }

    public override void OnFixedUpdate() 
    {
        PositionDialogBoxes();
    }

    protected void DialogOptionSelected(int index)
    {
        awaitingResponse = false;
        flowPlayer.Play(dialogOptions[index]);
    }
}
