using Articy.Harmonybarktest;
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
        public ArticyFlowPlayer flowPlayer;
        public MelodyController melodyController;
        public GameObject dialogOptionHolder;

        [HideInInspector]
        public bool inDialog = false;

        private List<DialogSpeakerNPC> speakers;
        private const float spacing = 150;
        private DialogSpeakerNPC proximalSpeaker;
        private IList<Branch> dialogOptions;
        private Branch dialog;
        private bool awaitingResponse = false;

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
                        Speak(speaker.Speaker.TechnicalName, text.Text);
                    }
                }
            }
        }

        public void OnFlowPlayerPaused(IFlowObject aObject) { }

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
            proximalSpeaker = GetHighestScoredDialogTarget();
            if (ServiceLocator.instance.GetInputManager().InteractButtonDown() && !awaitingResponse)
            {
                if (inDialog)
                {
                    flowPlayer.Play(dialog);
                    flowPlayer.FinishCurrentPausedObject();
                }
                else
                {
                    if (proximalSpeaker != null && proximalSpeaker.DialogReference.HasReference)
                    {
                        inDialog = true;
                        StartDialog(proximalSpeaker.DialogReference);
                    }
                }
            }
        }

        private DialogSpeakerNPC GetHighestScoredDialogTarget()
        {
            DialogSpeakerNPC highestScoredDialogTarget = null;

            float highestScore = 0f;
            float score = 0f;

            float angle = 0f;
            float angleScore = 0f;
            float angleScoreWeight = 0.4f;
            float maxAngle = 60f;

            float distance = 0f;
            float distanceScore = 0f;
            float distanceScoreWeight = 0.6f;
            float maxDistance = 5;

            foreach (DialogSpeakerNPC speaker in speakers)
            {
                speaker.dialogView.activeSpeaker.SetActive(false);
                if (speaker.playerInRange)
                {
                    distance = Vector3.Distance(ServiceLocator.instance.GetMelodyController().transform.position, speaker.transform.position);

                    distanceScore = (Mathf.Max(maxDistance - distance, 0f) / maxDistance) * distanceScoreWeight;

                    angle = GetPotentialTargetAngleWorldSpace(speaker.transform.position);
                    angleScore = ((maxAngle - angle) / maxAngle) * angleScoreWeight;

                    score = angleScore + distanceScore;

                    if (score > highestScore && angle < maxAngle)
                    {
                        highestScore = score;
                        highestScoredDialogTarget = speaker;
                    }
                }
            }
            if (highestScoredDialogTarget != null && highestScoredDialogTarget.DialogReference.HasReference && !inDialog)
            {
                highestScoredDialogTarget.dialogView.activeSpeaker.SetActive(true);
            }
            return highestScoredDialogTarget;
        }

        public float GetPotentialTargetAngleWorldSpace(Vector3 targetPos)
        {
            //Calculate the angle of the target by getting the angle between the target position relative to the player, and the direction the player is facing.
            Vector3 sourceDirection = targetPos - ServiceLocator.instance.GetMelodyController().transform.position;
            return Vector3.Angle(ServiceLocator.instance.GetMelodyController().transform.forward, sourceDirection);
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
