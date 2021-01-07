using Articy.Unity;
using Articy.Unity.Interfaces;
using GameManager;
using Melody;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HarmonyQuest.Dialog
{
    public class DialogController : ManageableObject, IArticyFlowPlayerCallbacks
    {
        private List<DialogSpeakerNPC> speakers;
        private const float spacing = 150;
        private ArticyFlowPlayer flowPlayer;
        private ArticyRef proximalDialog;
        private IList<Branch> dialogOptions;
        private Branch dialog;
        private bool awaitingResponse = false;
        private bool isBark = false;

        public bool inDialog = false;

        public MelodyController melodyController;

        public GameObject dialogOptionHolder;

        public DialogController()
        {
            speakers = new List<DialogSpeakerNPC>(5);
        }

        public bool RegisterSpeaker(DialogSpeakerNPC speaker)
        {
            if (!speakers.Contains(speaker))
            {
                speakers.Add(speaker);
                return true;
            }
            return false;
        }

        public void StartDialog(ArticyRef dialogRef)
        {
            if (dialogRef.GetObject() != null)
            {
                flowPlayer.StartOn = dialogRef.GetObject();
                melodyController.FreezeMovement();
                PauseManager.ToggleDialog(true);
                PlayerControllerStateManager.instance.SetState(PlayerControllerStateManager.ControllerState.Dialog);
            }
        }

        public void StartBark(ArticyRef barkRef)
        {
            if (barkRef.GetObject() != null)
            {
                isBark = true;
                flowPlayer.StartOn = barkRef.GetObject();
            }
        }

        public void EnterSpeakerZone(DialogSpeakerNPC speaker)
        {
            if (proximalDialog != speaker.DialogReference)
            {
                proximalDialog = speaker.DialogReference;
            }
        }


        public void ExitSpeakerZone(DialogSpeakerNPC speaker)
        {
            if (proximalDialog == speaker.DialogReference)
            {
                proximalDialog = null;
            }
        }

        public void Speak(string speakerTechnicalName, string text)
        {
            foreach (DialogSpeakerNPC speaker in speakers)
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

        public void SpeakBark(string speakerTechnicalName, string text)
        {
            foreach (DialogSpeakerNPC speaker in speakers)
            {
                if (speaker.character.GetObject().TechnicalName == speakerTechnicalName)
                {
                    speaker.SpeakBark(text);
                }
            }
        }

        public void EndDialog()
        {
            foreach (DialogSpeakerNPC speaker in speakers)
            {
                speaker.ShutUp();
            }
            melodyController.UnfreezeMovement();
            PauseManager.ToggleDialog(false);
            PlayerControllerStateManager.instance.SetState(PlayerControllerStateManager.ControllerState.Melody);
        }

        public void PositionDialogBoxes()
        {
            //Get Average X and Y of Speakers in screen coordinates
            Vector2 average = new Vector2();
            foreach (DialogSpeakerNPC speaker in speakers)
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
            foreach (DialogSpeakerNPC speaker in speakers)
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
                if (isBark)
                {
                    throw new System.Exception("Error: Barks cannot have more than one branch. Check the articy project to fix this.");
                }
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
                    flowPlayer.Play(dialog);
                    flowPlayer.FinishCurrentPausedObject();
                    flowPlayer.StartOn = null;
                    inDialog = false;
                    ServiceLocator.instance.GetDialogController().EndDialog();
                }
                else
                {
                    var text = dialog.Target as IObjectWithText;
                    var speaker = dialog.Target as IObjectWithSpeaker;
                    if (speaker != null)
                    {
                        if (isBark)
                        {
                            ServiceLocator.instance.GetDialogController().SpeakBark(speaker.Speaker.TechnicalName, text.Text);
                            flowPlayer.FinishCurrentPausedObject();
                            flowPlayer.StartOn = null;
                        }
                        else
                        {
                            ServiceLocator.instance.GetDialogController().Speak(speaker.Speaker.TechnicalName, text.Text);
                        }
                    }
                }
            }
            isBark = false;
        }

        public void OnFlowPlayerPaused(IFlowObject aObject) { }

        public override void OnAwake()
        {
            flowPlayer = FindObjectOfType<ArticyFlowPlayer>();

        }

        public override void OnStart()
        {
            melodyController = ServiceLocator.instance.GetMelodyController();

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
                    if (proximalDialog != null && proximalDialog.HasReference)
                    {
                        inDialog = true;
                        StartDialog(proximalDialog);
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
}
