using Articy.Harmonybarktest;
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

        private Camera mainCamera;
        private bool playerInRange = false;
        private Branch dialog;
        private DialogueType barkTypeFeature;

        public override void OnStart()
        {
            base.OnStart();

            if (DialogReference != null)
            {

                teaseCollider.AssignFunctionToTriggerEnterDelegate(TriggerEnter);
                teaseCollider.AssignFunctionToTriggerExitDelegate(TriggerExit);

                ServiceLocator.instance.GetDialogController().RegisterSpeaker(this);
                mainCamera = ServiceLocator.instance.GetCamera();

                if (DialogReference.HasReference)
                {
                    dialogView.bark.SetActive(false);
                    dialogView.indicator.SetActive(true);
                }
            }

            if (BarkReference != null)
            {
                //StartBark(BarkReference);
                //dialogView.indicator.SetActive(true);
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
                if (BarkReference != null && BarkReference.HasReference)
                {
                    //Delay the bark until the end of the frame so that the current dialogue gets closed out properly before reopening the flow manager.
                    StartCoroutine(DelayedBark());
                }
            }
            else
            {
                dialogView.indicator.SetActive(true);
            }
        }

        private IEnumerator DelayedBark()
        {
            yield return new WaitForEndOfFrame();
            StartBark(BarkReference);
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
            if (BarkReference != null && BarkReference.HasReference)
            {
                StartBark(BarkReference);
                dialogView.indicator.SetActive(false);
                ServiceLocator.instance.GetDialogController().EnterSpeakerZone(this);
                playerInRange = true;
            }
        }

        private void TriggerExit(Collider other)
        {
            if (BarkReference != null && BarkReference.HasReference)
            {
                dialogView.bark.SetActive(false);
                dialogView.indicator.SetActive(true);
                ServiceLocator.instance.GetDialogController().ExitSpeakerZone(this);
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

        public void SpeakBark(string speakerTechnicalName, string text)
        {
            if (character.GetObject().TechnicalName == speakerTechnicalName)
            {
                dialogView.barkText.text = text;
                dialogView.bark.SetActive(true);
            }
        }

        public void SetAssets(string speakerTechnicalName, DialogueType dialogueType)
        {
            if (character.GetObject().TechnicalName == speakerTechnicalName)
            {
                dialogView.SetAssets(dialogueType);
            }
        }

        public void OnBranchesUpdated(IList<Branch> aBranches)
        {
            if (aBranches.Count > 1)
            {
                throw new System.Exception("Error: Barks cannot have more than one branch. Check the articy project to fix this.");
            }
            else if (aBranches.Count == 1)
            {
                dialog = aBranches[0];
                var text = dialog.Target as IObjectWithText;
                var speaker = dialog.Target as IObjectWithSpeaker;
                if (speaker != null)
                {
                    var barkType = dialog.Target as BarkType;
                    if (barkType != null)
                    {
                        barkTypeFeature = barkType.Template.BARK_TYPE.DialogueType;
                        SetAssets(speaker.Speaker.TechnicalName, barkTypeFeature);
                    }
                    SpeakBark(speaker.Speaker.TechnicalName, text.Text);
                    flowPlayer.FinishCurrentPausedObject();
                    flowPlayer.StartOn = null;
                }
            }
        }

        public void OnFlowPlayerPaused(IFlowObject aObject) { }
    }
}