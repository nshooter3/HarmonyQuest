using Articy.Unity;
using Articy.Unity.Interfaces;
using GamePhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HarmonyQuest.Dialog
{
    public class DialogSpeakerNPC : DialogSpeaker, IArticyFlowPlayerCallbacks
    {
        public ArticyFlowPlayer flowPlayer;
        public ArticyRef DialogReference;
        public ArticyRef BarkReference;
        public CollisionWrapper teaseCollider;

        [HideInInspector]
        public bool playerInRange = false;

        private Camera mainCamera;
        private Branch dialog;
        private DialogView.DialogueType dialogueType;

        private bool initIndicator;

        public override void OnStart()
        {
            base.OnStart();

            teaseCollider.AssignFunctionToTriggerEnterDelegate(TriggerEnter);
            teaseCollider.AssignFunctionToTriggerExitDelegate(TriggerExit);

            ServiceLocator.instance.GetDialogController().RegisterSpeaker(this);
            mainCamera = ServiceLocator.instance.GetCamera();

            if (BarkReference.HasReference)
            {
                initIndicator = true;
                StartBark(BarkReference);
            }
        }

        public override void Speak(string text)
        {
            base.Speak(text);
            dialogView.bark.SetActive(false);
        }

        public override void ShutUp()
        {
            base.ShutUp();

            if (playerInRange)
            {
                if (BarkReference.HasReference)
                {
                    StartBark(BarkReference);
                }
            }
            else
            {
                if (BarkReference.HasReference)
                {
                    initIndicator = true;
                    StartBark(BarkReference);
                }
            }
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

        private void TriggerEnter(Collider other)
        {
            if (BarkReference.HasReference)
            {
                StartBark(BarkReference);
                dialogView.indicator.SetActive(false);
                playerInRange = true;
            }
        }

        private void TriggerExit(Collider other)
        {
            if (BarkReference.HasReference)
            {
                dialogView.bark.SetActive(false);
                if (dialogView.useIndicator)
                {
                    dialogView.indicator.SetActive(true);
                }
                playerInRange = false;
            }
        }

        public void StartBark(ArticyRef barkRef)
        {
            if (barkRef.GetObject() != null)
            {
                flowPlayer.StartOn = barkRef.GetObject();
            }
        }

        public void SpeakBark(string text)
        {
            dialogView.barkText.text = text;
            if (initIndicator)
            {
                if (dialogView.useIndicator)
                {
                    dialogView.indicator.SetActive(true);
                }
                initIndicator = false;
            }
            else
            {
                dialogView.bark.SetActive(true);
            };
        }

        public void SetAssets(DialogView.DialogueType dialogueType)
        {
            dialogView.SetAssets(dialogueType);
        }

        public void OnBranchesUpdated(IList<Branch> aBranches)
        {
            if (aBranches.Count > 1)
            {
                foreach (Branch branch in aBranches)
                {
                    var bark = dialog.Target as IObjectWithStageDirections;
                    if (bark != null)
                    {
                        Debug.Log("BARK: " + bark.StageDirections);
                    }

                    var text = branch.Target as IObjectWithText;
                    if (text != null)
                    {
                        Debug.Log("TEXT: " + text.Text);
                    }
                }
                throw new System.Exception("Error: Barks cannot have more than one branch. Check the articy project to fix this.");
            }
            else if (aBranches.Count == 1)
            {
                dialog = aBranches[0];
                var bark = dialog.Target as IObjectWithStageDirections;
                if (bark != null)
                {
                    if (DialogReference.HasReference)
                    {
                        //Asterisks are the chars we use to identify barks that should be marked as quest related.
                        if (bark.StageDirections != null && bark.StageDirections[0] == '*')
                        {
                            dialogueType = DialogView.DialogueType.Quest;
                        }
                        else
                        {
                            dialogueType = DialogView.DialogueType.Talk;
                        }
                    }
                    else
                    {
                        dialogueType = DialogView.DialogueType.NonInteractive;
                    }
                    SetAssets(dialogueType);

                    SpeakBark(bark.StageDirections);
                    flowPlayer.FinishCurrentPausedObject();
                    flowPlayer.StartOn = null;
                }
            }
        }

        public void OnFlowPlayerPaused(IFlowObject aObject) { }
    }
}